using Assets._VRN.Core.Runtime.Utils;
using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRN.Utils;

public class RespawnInteractableObjects : MonoBehaviour
{
    [SerializeField] private BoxCollider interactableArea;
    public float respawnTime;
    [SerializeField] private float dissolveEffectDuration = 0.5f;

    private bool _outOfArea;
    private bool _grabbing;

    private Rigidbody _rigidbody;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private int _dissolvePropertyId;
    private List<Material> _materials = new List<Material>();

    private Coroutine _coroutineRespawn;

    private bool _forceOutOfArea;
    public BoxCollider InteractableArea => interactableArea;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _dissolvePropertyId = Shader.PropertyToID("_DissolveAmount");
        // Retrieve materials from hierarchy
        var meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in meshRenderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty(_dissolvePropertyId))
                {
                    renderer.materials[i] = Instantiate<Material>(renderer.materials[i]);
                    _materials.Add(renderer.materials[i]);
                }
            }
        }

        RespawnInit();
        _outOfArea = false;

        var g = GetComponent<Grabbable>();
        if (g)
        {
            g.onGrab.AddListener(GrabbingEvent);
            g.onRelease.AddListener(ReleasingEvent);
        }

        var gi = GetComponent<XRGrabInteractable>();
        if (gi)
        {
            gi.selectEntered.AddListener((x) =>
            {
                GrabbingEvent(null, null);
            });
            gi.selectExited.AddListener((x) =>
            {
                ReleasingEvent(null, null);
            });
        }

        Debug.Assert(g || gi);
    }

    public void RespawnInit()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }

    private void ReleasingEvent(Hand arg0, Grabbable arg1)
    {
        _grabbing = false;
        _forceOutOfArea = interactableArea == null;
    }

    private void GrabbingEvent(Hand arg0, Grabbable arg1)
    {
        _grabbing = true;
        if (_forceOutOfArea)
        {
            _forceOutOfArea = false;
            RespawnStop();
        }
    }

    private void Update()
    {
        if ((_outOfArea || _forceOutOfArea) && !_grabbing)
        {
            Respawn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == interactableArea)
        {
            _outOfArea = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == interactableArea)
        {
            _outOfArea = false;
            RespawnStop();
        }
    }

    private void SetDissolveParameter(float value)
    {
        foreach (var material in _materials)
        {
            material.SetFloat(_dissolvePropertyId, value);
        }
    }

    private IEnumerator Dissolve(float duration)
    {
        float percent = 0;
        float timeFactor = 1 / (duration * 0.5f);
        while (percent < 1)
        {
            percent += Time.deltaTime * timeFactor;
            var value = Mathf.Lerp(0, 1, percent);
            SetDissolveParameter(value);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        Respawn();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * timeFactor;
            var value = Mathf.Lerp(1, 0, percent);
            SetDissolveParameter(value);
            yield return null;
        }
    }

    public void Respawn()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = _initialPosition;
        transform.localRotation = _initialRotation;
    }

    public void RespawnStop()
    {
        if (_coroutineRespawn != null)
        {
            StopCoroutine(_coroutineRespawn);
            _coroutineRespawn = null;
        }
    }

    public bool IsRespawning()
    {
        return _coroutineRespawn == null;
    }

    public void Respawn(float duration, bool force = false)
    {
        if (force == false && IsRespawning())
        {
            return;
        }
        RespawnStop();
        if (duration <= 0f)
        {
            Respawn();
        }
        _coroutineRespawn = CoroutineUtil.Do(Dissolve(dissolveEffectDuration), respawnTime);
    }
}