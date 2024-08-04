using System.Collections;
using System.Collections.Generic;
using Assets._VRN.Core.Runtime.StateMachine;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using VRN.Modules;
using VRN.Utils;

public class EPPItem : MonoBehaviour
{
    public static List<EPPItem> instances = new();

    // NOTE: interact only with one object at a time,
    // made it this way so we don't have to handle how what happens when the user grabs two interactables at the same time,
    // it mostly caused visibility conflitcs, like, showing renderers that clipped (socket fresnel, item positionated, etc)
    private static XRGrabInteractable currentGrabInteractable;

    private static readonly int PARAM_IS_PLACED = Animator.StringToHash("isPlaced");

    [Header("Grid Properties")]
    public float scale = 1f;
    [SerializeField]
    private float separationDistance;
    [SerializeField]
    private float textHeightOffset;

    [HideInInspector]
    public GameObject textGO;
    [HideInInspector]
    public Vector3 originalScale;
    [HideInInspector]
    public float transitionDuration;

    [Space]
    public EPPItemSocket socket;

    public bool outlineEnabled = true;

    [SerializeField]
    private GameObject synced;

    [ReadOnly]
    public Collider[] collsPlaced;

    [ReadOnly]
    public Collider[] collsUnplaced;

    private Animator anim;
    private RespawnInteractableObjects respawn;
    private ItemPositioner positioner;
    private XRGrabInteractable grabInteractable;
    private Transform followTarget;
    private Vector3 followLocalPos;
    private Renderer[] renderers;

    private bool _highlighted;
    private Coroutine _transitionCoroutine;
    private Vector3 _gridScale;
    private Transform _textContainer;
    private RectTransform _itemContainer;

    [field: SerializeField]
    public Outline Outline { get; private set; }

    public bool IsPositionated => positioner && positioner.ItemPositioned;

    private class XRFilterUniqueInteractor : IXRHoverFilter, IXRSelectFilter
    {
        public bool canProcess => true;

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            return
                (currentGrabInteractable == null || currentGrabInteractable == interactable) &&
                interactable.isHovered == false || interactable.interactorsHovering.Contains(interactor);
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return
                (currentGrabInteractable == null || currentGrabInteractable == interactable) &&
                interactable.isSelected == false || interactable.interactorsSelecting.Contains(interactor);
        }
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        respawn = GetComponent<RespawnInteractableObjects>();
        positioner = gameObject.AddComponent<ItemPositioner>();

        collsPlaced = synced.GetComponentsInChildren<Collider>();
        collsUnplaced = GetComponents<Collider>();

        Outline = GetComponent<Outline>();

        for (int i = 0; i < collsPlaced.Length; i++)
        {
            collsPlaced[i].enabled = false;
        }

        respawn.respawnTime = 0f;

        positioner.ItemSocketGameObject = socket.gameObject;
        positioner.ItemSocketCheckAttachedRigidobdy = true;
        positioner.withItemPositioned = false;

        positioner.OnPosition.AddListener((p) =>
        {
            PlacedUpdate(true);
            positioner.ItemSocketShow(false);
            UnpositionOther(this);
        });
        positioner.OnPositionUndo.AddListener((p) =>
        {
            PlacedUpdate(false);
            positioner.ItemSocketShow(false);
            Show(true);
        });
        positioner.OnShow.AddListener((p) =>
        {
            socket.ShowMannequinParts(false);
        });
        positioner.OnHide.AddListener((p) =>
        {
            socket.ShowMannequinParts(positioner.ItemPositioned == false && positioner.ItemInZone == false);
        });

        positioner.OnEnterZone.AddListener((p) =>
        {
            PlacedUpdateVisualsOnly(true);
            positioner.ItemSocketShow(false);
        });
        positioner.OnExitZone.AddListener((p) =>
        {
            if (p.ItemPositioned == false)
            {
                PlacedUpdateVisualsOnly(false);
                positioner.ItemSocketShow(true);
            }
        });

        _gridScale = this.transform.localScale;
        _textContainer = textGO.transform.parent;
        _textContainer.gameObject.SetActive(false);
        _itemContainer = this.transform.parent.parent.GetComponent<RectTransform>();

        var textHeight = _textContainer.localPosition;
        textHeight.y += textHeightOffset;
        _textContainer.localPosition = textHeight;
    }

    private void LateUpdate()
    {
        if (grabInteractable.isSelected)
        {
            grabInteractable.transform.position = followTarget.TransformPoint(followLocalPos);
        }
    }

    private void Awake()
    {
        if (instances.Count == 0)
        {
            currentGrabInteractable = null;
            ModuleManager.Singleton.OnModuleFinished += OnModuleFinished;
            ApplicationStateMachine.Singleton.OnEnterState.AddListener(OnEnterState);
        }
        instances.Add(this);

        renderers = synced.GetComponentsInChildren<Renderer>();

        grabInteractable = gameObject.AddComponent<XRGrabInteractable>();

        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;
        grabInteractable.throwOnDetach = false;
        grabInteractable.retainTransformParent = true;
        var xrFilterUniqueInteractor = new XRFilterUniqueInteractor();
        grabInteractable.hoverFilters.Add(xrFilterUniqueInteractor);
        grabInteractable.selectFilters.Add(xrFilterUniqueInteractor);

        grabInteractable.selectEntered.AddListener((x) =>
        {
            _textContainer.gameObject.SetActive(false);

            currentGrabInteractable = grabInteractable;
            followTarget = x.interactorObject.transform;
            followLocalPos = followTarget.InverseTransformPoint(transform.position);
            followLocalPos.x = 0;
            followLocalPos.y = 0;
            PlacedUpdate(false);
            if (positioner.ItemPositioned == false)
            {
                respawn.RespawnInit();
                ShowOther(false);
            }
            EPPModelVariant.Show(socket.modelVariants, true);
        });
        grabInteractable.selectExited.AddListener((x) =>
        {
            currentGrabInteractable = null;
            CoroutineUtil.Do(null, () =>
            {
                if (positioner.ItemPositioned == false)
                {
                    EPPModelVariant.Show(socket.modelVariants, false);
                    ShowOther(true);

                    if(_highlighted)
                        _textContainer.gameObject.SetActive(true);
                }
            });
        });
        grabInteractable.hoverEntered.AddListener(x =>
        {
            if (outlineEnabled == false)
            {
                return;
            }
            Outline.enabled = true;
            Outline.OutlineColor = Color.white;

            HighlightObject();
        });
        grabInteractable.hoverExited.AddListener(x =>
        {
            if (outlineEnabled == false)
            {
                return;
            }
            Outline.enabled = false;

            LeaveHighlight();
        });
    }

    private void HighlightObject()
    {
        if (!grabInteractable.isSelected && !this.IsPositionated)
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }
            _transitionCoroutine = StartCoroutine(TransitionToHighlight(true));
        }
    }

    private void LeaveHighlight()
    {
        if (!grabInteractable.isSelected && _highlighted)
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }
            _transitionCoroutine = StartCoroutine(TransitionToHighlight(false));
        }
    }
    private IEnumerator TransitionToHighlight(bool highlight)
    {
        _highlighted = highlight;
        _textContainer.gameObject.SetActive(highlight);

        Vector3 startScale = this.transform.localScale;
        Vector3 endScale = highlight ? originalScale : _gridScale;

        Vector3 startPosition = _itemContainer.parent.localPosition;
        Vector3 endPosition = highlight ? new Vector3(0, 0, startPosition.z - separationDistance) : Vector3.zero;

        Quaternion startRotation = _itemContainer.localRotation;
        Quaternion endRotation = highlight ? Quaternion.Euler(0, 30, 0) : Quaternion.Euler(0, 0, 0);

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            this.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / transitionDuration);
            _itemContainer.parent.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);
            _itemContainer.localRotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / transitionDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.transform.localScale = endScale;
        _itemContainer.parent.localPosition = endPosition;
        _itemContainer.localRotation = endRotation;
    }

    private void OnDestroy()
    {
        instances.Remove(this);
        if (instances.Count == 0)
        {
            ModuleManager.Singleton.OnModuleFinished -= OnModuleFinished;
            ApplicationStateMachine.Singleton.OnEnterState.RemoveListener(OnEnterState);
        }
        if (positioner)
        {
            positioner.OnPosition.RemoveAllListeners();
            positioner.OnPositionUndo.RemoveAllListeners();
        }
        if (grabInteractable)
        {
            grabInteractable.selectEntered.RemoveAllListeners();
            grabInteractable.selectExited.RemoveAllListeners();
            grabInteractable.hoverEntered.RemoveAllListeners();
            grabInteractable.hoverExited.RemoveAllListeners();
            grabInteractable.hoverFilters.Clear();
        }
    }

    private void OnEnterState(StateEnum to, StateEnum from, object x)
    {
        if (to == StateEnum.ModulePaused)
        {
            for (int i = 0; i < instances.Count; i++)
            {
                var ins = instances[i];
                ins.grabInteractable.enabled = false;
            }
        }
        if (from == StateEnum.ModulePaused)
        {
            for (int i = 0; i < instances.Count; i++)
            {
                var ins = instances[i];
                ins.grabInteractable.enabled = true;
            }
        }
    }

    private void OnModuleFinished(ModuleManager mngr, ModuleAsset module, ModuleManager.ResultEnum result, int code)
    {
        for (int i = 0; i < instances.Count; i++)
        {
            var ins = instances[i];
            ins.grabInteractable.enabled = false;
        }
    }

    private void PlacedUpdate(bool isPlaced)
    {
        if (respawn)
        {
            respawn.enabled = !isPlaced;
            if (respawn.enabled)
            {
                respawn.Respawn();
            }
        }
        foreach (var coll in collsPlaced)
        {
            coll.enabled = isPlaced;
        }
        foreach (var coll in collsUnplaced)
        {
            coll.enabled = !isPlaced;
        }
        PlacedUpdateVisualsOnly(isPlaced);
        if (isPlaced)
        {
            synced.transform.parent = transform;
            synced.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    private void PlacedUpdateVisualsOnly(bool isPlaced)
    {
        if (isPlaced)
        {
            anim.SetBool(PARAM_IS_PLACED, true);
            var tr = positioner.GetItemSocketTransform();
            synced.transform.parent = null;
            synced.transform.SetPositionAndRotation(tr.position, tr.rotation);
        }
        else
        {
            anim.SetBool(PARAM_IS_PLACED, false);
            synced.transform.parent = transform;
            synced.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    [Button("Toggle Position", EButtonEnableMode.Playmode)]
    public void PositionObject()
    {
        positioner.PositionObject(!positioner.ItemPositioned);
    }

    [Button("Sync", EButtonEnableMode.Editor)]
    public void Sync()
    {
        if (socket == null || socket.syncParent == null)
        {
            Debug.LogError("socket == null || socket.syncParent == null");
            return;
        }
        name = socket.name.Replace("-SOCKET", "-INTERACTABLE");
        if (synced)
        {
            DestroyImmediate(synced.gameObject);
        }
        synced = Instantiate(socket.syncParent.gameObject, transform);
        synced.name = socket.syncParent.name;
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
            collider.isTrigger = false;
        }
    }

    private static void UnpositionOther(EPPItem epp)
    {
        for (int i = 0; i < instances.Count; i++)
        {
            var other = instances[i];
            if (other != epp &&
                other.positioner.ItemPositioned &&
                EPPItemSocket.Intersects(other.socket.tags, epp.socket.tags))
            {
                //FindObjectOfType<EPPUI>().Show(other, true);
                other.positioner.PositionObject(false);
                return;
            }
        }
    }

    private void ShowOther(bool show)
    {
        for (int i = 0; i < instances.Count; i++)
        {
            var other = instances[i];
            if (other != this &&
                other.positioner.ItemPositioned &&
                EPPItemSocket.Intersects(other.socket.tags, socket.tags))
            {
                other.Show(show);
            }
        }
    }

    public void Show(bool show)
    {
        Debug.Log($"EPP | Show: {show}, {name}", this);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = show;
        }
    }
}
