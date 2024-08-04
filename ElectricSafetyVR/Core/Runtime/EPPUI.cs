using Assets._VRN.Core.Runtime.UI;
using TMPro;
using UnityEngine;
using VRN.Modules;

public class EPPUI : MonoBehaviour
{
    public GameObject itemPrefab;
    public string pivotPath = "Panel/Pivot";
    [SerializeField]
    public float transitionDuration;

    [SerializeField]
    private TMP_Text exerciseTitle;

    private void Start()
    {
        exerciseTitle.text = ModuleManager.Singleton.Current.DisplayDescription;

        foreach (var epp in EPPItem.instances)
        {
            var ins = Instantiate(itemPrefab, itemPrefab.transform.parent);
            ins.SetActive(true);
            ins.name = "Item: " + epp.name;

            ins.GetComponentInChildren<TextMeshProUGUI>().text = epp.socket.displayName.GetLocalizedString();
            epp.textGO = ins.GetComponentInChildren<TextMeshProUGUI>().gameObject;

            var pivot = ins.transform.Find(pivotPath);
            epp.transform.SetParent(pivot.transform, true);
            epp.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 180f, 0f));

            var respanwer = epp.GetComponent<RespawnInteractableObjects>();
            respanwer.RespawnInit();

            epp.originalScale = epp.transform.localScale;
            epp.transform.localScale *= epp.scale;
            epp.transitionDuration = transitionDuration;
        }
        itemPrefab.SetActive(false);
        var ui = GetComponent<IUIOpenable>();
        if (ui != null)
        {
            ui.onOpen += Open;
            ui.onClose += Close;
            for (int i = 0; i < EPPItem.instances.Count; i++)
            {
                EPPItem.instances[i].Show(ui.IsOpen);
            }
        }
    }

    private void OnDestroy()
    {
        var ui = GetComponent<IUIOpenable>();
        if (ui != null)
        {
            ui.onOpen -= Open;
            ui.onClose -= Close;
        }
    }

    private void Open()
    {
        for (int i = 0; i < EPPItem.instances.Count; i++)
        {
            EPPItem.instances[i].Show(true);
        }
    }

    private void Close()
    {
        for (int i = 0; i < EPPItem.instances.Count; i++)
        {
            if (!EPPItem.instances[i].IsPositionated) // HACK: para evitar que se oculten los objetos que se sacaron de la grilla al finalizar el ejercicio
            {  
                EPPItem.instances[i].Show(false);
            }
        }
    }
}
