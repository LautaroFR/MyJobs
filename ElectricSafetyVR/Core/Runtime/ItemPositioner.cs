using VRN.GOIDs;
using Autohand;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Serialization;
using NaughtyAttributes;

[RequireComponent(typeof(Grabbable))]
[RequireComponent(typeof(RespawnInteractableObjects))]

public class ItemPositioner : MonoBehaviour
{
    // NOTE: @fdzeppo - se podria usar SharedGameObject.GameObjectOrGOIDAsset,
    // por ahi puede dejar de ser subclase, en su momento lo hice asi para que no nos equivoquemos y
    // usemos siempre SharedGameObject en las subclases de BehaviorDesigner
    public GOIDAsset PositionedItemAsset;
    public GameObject PositionedItem;

    [Space]
    public GOIDAsset ItemSocketAsset;
    [FormerlySerializedAs("ItemSocket")]
    public Collider ItemSocketCollider;
    public GameObject ItemSocketGameObject;
    public bool ItemSocketCheckAttachedRigidobdy;

    [HideInInspector]
    [ShowNonSerializedField]
    public bool ItemPositioned;

    [Space]
    [SerializeField]
    private bool autorelease;

    [SerializeField]
    private bool instantlyRespawn;

    // NOTE: @fdzeppo - este boolean podria no existir o autoconfigurarse cuando se le asigna PositionedItem
    public bool withItemPositioned;

    [ShowNonSerializedField] 
    private bool _itemInZone;

    private Grabbable _grabbableItem;
    private XRGrabInteractable _grabInteractable;

    public UnityEvent<ItemPositioner> OnPosition = new UnityEvent<ItemPositioner>();
    public UnityEvent<ItemPositioner> OnPositionUndo = new UnityEvent<ItemPositioner>();

    public UnityEvent<ItemPositioner> OnEnterZone = new UnityEvent<ItemPositioner>();
    public UnityEvent<ItemPositioner> OnExitZone = new UnityEvent<ItemPositioner>();

    public UnityEvent<ItemPositioner> OnShow = new UnityEvent<ItemPositioner>();
    public UnityEvent<ItemPositioner> OnHide = new UnityEvent<ItemPositioner>();

    private RespawnInteractableObjects _respawner;

    public bool ItemInZone => this._itemInZone;

    void Start()
    {
        _grabbableItem = this.GetComponent<Grabbable>();
        _grabbableItem.onGrab.AddListener(ItemGrabbed);
        _grabbableItem.onRelease.AddListener(ItemDropped);

        _grabInteractable = this.GetComponent<XRGrabInteractable>();
        if (_grabInteractable)
        {
            _grabInteractable.selectEntered.AddListener((x) =>
            {
                ItemGrabbed(null, null);
            });
            _grabInteractable.selectExited.AddListener((x) =>
            {
                ItemDropped(null, null);
            });
        }

        _respawner = GetComponent<RespawnInteractableObjects>();

        if (ItemSocketCollider == null && ItemSocketAsset == null && ItemSocketGameObject == null)
            Debug.LogError("[ItemPositioner] No ItemSocket asigned");

        if (withItemPositioned)
        {
            if (PositionedItem == null)
            {
                if (PositionedItemAsset == null)
                    Debug.LogError("[ItemPositioner] No PositionedItem asigned");
                else
                    GOID.TryFind(PositionedItemAsset, out PositionedItem);
            }
        }

        if (ItemSocketCollider == null && ItemSocketGameObject == null)
        {
            GOID.TryFind(ItemSocketAsset, out var itemSocket);
            if (itemSocket == null)
            {
                Debug.LogError("[ItemPositioner] No ItemSocket found by GOID");
                return;
            }
            ItemSocketCollider = itemSocket.GetComponent<Collider>();
        }

        Debug.Assert(ItemSocketCollider != null || ItemSocketGameObject != null);

        ItemSocketShow(false);

        //TODO: Verificar si es necesario refactorear esto
        //if (this._grabbableItem.isSelected)
        //{
        //    this.ItemGrabbed(new SelectEnterEventArgs());
        //}
        //else
        //{
        //    this.ItemDropped(new SelectExitEventArgs());
        //}
    }

    private void ItemDropped(Hand arg0, Grabbable arg1)
    {
        ItemSocketShow(false);

        if (_itemInZone)
            PositionObject(true);

        if (!_itemInZone && instantlyRespawn)
            _respawner.Respawn(0.3f);
    }

    private void ItemGrabbed(Hand arg0, Grabbable arg1)
    {
        ItemSocketShow(true);
        PositionObject(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TriggerCheck(other))
        {
            _itemInZone = true;
            this.OnEnterZone.Invoke(this);

            if (autorelease)
                PositionObject(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (TriggerCheck(other))
        {
            _itemInZone = false;
            this.OnExitZone.Invoke(this);
        }
    }

    private bool TriggerCheck(Collider other)
    {
        return
            other == ItemSocketCollider ||
            ItemSocketGameObject != null && other.gameObject == ItemSocketGameObject ||
            ItemSocketCheckAttachedRigidobdy &&
            (
                other.attachedRigidbody != null &&
                (
                    ItemSocketCollider != null && other.attachedRigidbody == ItemSocketCollider.attachedRigidbody ||
                    ItemSocketGameObject != null && other.attachedRigidbody.gameObject == ItemSocketGameObject
                )
            );
    }

    public void PositionObject(bool positionate)
    {
        if (ItemPositioned == positionate)
        {
            return;
        }
        Debug.Log($"EPP | ItemPositioner | PositionObject: {positionate}, {name}", this);
        if (positionate)
        {
            if (withItemPositioned)
            {
                PositionedItem.SetActive(true);
                this.gameObject.SetActive(false);
            }
            else
            {
                var tr = GetItemSocketTransform();
                this.transform.SetPositionAndRotation(tr.transform.position, tr.transform.rotation);
            }
            OnPosition?.Invoke(this);
        }
        else
        {
            if (withItemPositioned)
            {
                PositionedItem.SetActive(false);
                this.gameObject.SetActive(true);
            }
            OnPositionUndo?.Invoke(this);
        }
        ItemPositioned = positionate;
    }

    public void ItemSocketShow(bool show)
    {
        Debug.Log($"EPP | ItemPositioner | ItemSocketShow: {show}, {name}", this);
        var tr = GetItemSocketTransform();
        foreach (var r in tr.GetComponentsInChildren<Renderer>())
        {
            r.enabled = show;
        }
        if (show)
        {
            OnShow?.Invoke(this);
        }
        else
        {
            OnHide?.Invoke(this);
        }
    }

    public Transform GetItemSocketTransform()
    {
        Debug.Assert(ItemSocketCollider || ItemSocketGameObject);
        return ItemSocketCollider ? ItemSocketCollider.transform : ItemSocketGameObject.transform;
    }
}
