using System.Collections.Generic;

using Assets._VRN.Core.Runtime.XR;

using Autohand;
using UnityEngine;
using UnityEngine.Events;

public class FireExtinguisherController : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private float lifeTime = 60f;

    [SerializeField]
    private float decrementFactor;

    [SerializeField]
    private float coneAngle;

    [SerializeField]
    private Grabbable grabbable;

    [SerializeField]
    private GameObject safetyPin;

    [SerializeField]
    private ParticleSystem fireExtinguisherParticles;

    public UnityEvent<FireExtinguisherController> OnStartApplying = new UnityEvent<FireExtinguisherController>();

    public UnityEvent<FireExtinguisherController> OnStopApplying = new UnityEvent<FireExtinguisherController>();

    public UnityEvent<FireExtinguisherController> OnStartEmpty = new UnityEvent<FireExtinguisherController>();

    public UnityEvent<FireExtinguisherController> OnStopEmpty = new UnityEvent<FireExtinguisherController>();

    private bool _activating;
    private bool _fireInZone;
    private bool _forbiddenTarget;

    private FireManager _fire;
    private TargetController _targetController;

    private Transform _target;

    private float _actionAngle;

    private float currentLifeTime;

    public Grabbable Grabbable => grabbable;

    public bool IsApplying => _activating;

    public bool IsEmpty => currentLifeTime > lifeTime;

    public void Reset()
    {
        currentLifeTime = 0f;
    }

    private void OnEnable()
    {
        grabbable.onSqueeze.AddListener(ActivatingEvent);
        grabbable.onUnsqueeze.AddListener(PassiveEvent);

        _actionAngle = Mathf.Cos(coneAngle * Mathf.Deg2Rad);
    }

    private void OnDisable()
    {
        grabbable.onSqueeze.RemoveListener(ActivatingEvent);
        grabbable.onUnsqueeze.RemoveListener(PassiveEvent);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Avatar"))
        {
            _forbiddenTarget = true;
        }

            if (other.gameObject.GetComponent<FireManager>() != null)
        {
            _fire = other.GetComponent<FireManager>();
            _fireInZone = true;
            _target = _fire.transform;
            _forbiddenTarget = false;
        }

        if (other.gameObject.GetComponent<TargetController>() != null)
        {
            _targetController = other.GetComponent<TargetController>();
            _fireInZone = true;
            _target = _targetController.transform;
            _forbiddenTarget = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Avatar"))
        {
            _forbiddenTarget = false;
        }

        if (other.gameObject.GetComponent<FireManager>() != null)
        {
            _fireInZone = false;
            _fire = null;
            _target = null;
            StopApplying();
        }

        if (other.gameObject.GetComponent<TargetController>() != null)
        {
            _fireInZone = false;
            _targetController = null;
            _target = null;
        }
    }

    private void Update()
    {
        if (_activating && !_forbiddenTarget)
        {
            if (IsEmpty)
            {
                StopApplying();

                this.OnStartEmpty?.Invoke(this);

                return;
            }

            fireExtinguisherParticles?.Play();

            currentLifeTime += Time.deltaTime;

            if (_fireInZone)
            {
                Vector3 fireDirection = (_target.position - transform.position).normalized;

                var actionFactor = Vector3.Dot(transform.forward, fireDirection);

                float distance = Vector3.Distance(transform.position, _target.position);

                if (actionFactor > _actionAngle)
                {
                    var factor = (actionFactor * decrementFactor * Time.deltaTime) / distance;
                    if (_fire != null)
                        _fire.ReduceFire(factor);
                    if (_targetController != null)
                        _targetController.ApplyingFire(factor);
                }
            }
        }
    }

    private void StartApplying()
    {
        if (!_activating)
        {
            _activating = true;
            fireExtinguisherParticles?.Play();

            OnStartApplying?.Invoke(this);
        }
    }

    private void StopApplying()
    {
        if (_activating)
        {
            _activating = false;
            fireExtinguisherParticles?.Stop();
            OnStopApplying?.Invoke(this);
        }
    }

    #region GrabbingEvents
    private void ActivatingEvent(Hand arg0, Grabbable arg1)
    {
        if (!safetyPin.activeInHierarchy && !_forbiddenTarget)
        {
            if (IsEmpty)
            {
                OnStartEmpty?.Invoke(this);
            }
            else
            {
                StartApplying();
            }
        }
    }

    private void PassiveEvent(Hand arg0, Grabbable arg1)
    {
        if (IsEmpty)
        {
            OnStopEmpty?.Invoke(this);
        }
        else
        {
            StopApplying();
        }
    }
    #endregion
}