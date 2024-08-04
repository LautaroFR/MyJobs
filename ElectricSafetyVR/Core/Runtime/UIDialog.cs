using System.Collections;
using System.Collections.Generic;
using Assets._VRN.Core.Runtime.Utils;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.Localization.Components;

public class UIDialog : MonoBehaviour
{
    public enum Buttons
    {
        Yes,
        No,
        Cancel,
    }

    [SerializeField]
    private ButtonManager[] buttons;


    public void ButtonShow(Buttons buttonIndex, bool show)
    {
        var button = buttons[(int)buttonIndex];
        button.transform.parent.gameObject.SetActive(show);
    }


    public void ButtonSetLocalization(Buttons buttonIndex, string localizationTableName, string localizationEntryKey)
    {
        var button = buttons[(int)buttonIndex];
        var localizeStringEvent = button.GetComponent<LocalizeStringEvent>();
        localizeStringEvent.SetTable(localizationTableName);
        localizeStringEvent.SetEntry(localizationEntryKey);
    }
}
