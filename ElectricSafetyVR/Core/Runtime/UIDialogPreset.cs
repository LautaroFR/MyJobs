using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

[CreateAssetMenu(menuName = "VRN/ISVR/UI/Dialog Preset")]
public class UIDialogPreset : ScriptableObject
{
    public LocalizedString yesLocalization;
    
    [Space]
    public LocalizedString noLocalization;
    
    [Space]
    public LocalizedString cancelLocalization;

    public void ApplyPreset(UIDialog dialog)
    {
        var yesButtonShow =
            yesLocalization.TableReference.ReferenceType != TableReference.Type.Empty &&
            yesLocalization.TableEntryReference.ReferenceType != TableEntryReference.Type.Empty;
        dialog.ButtonShow(UIDialog.Buttons.Yes, yesButtonShow);
        if (yesButtonShow)
        {
            dialog.ButtonSetLocalization(UIDialog.Buttons.Yes, yesLocalization.TableReference.TableCollectionName, yesLocalization.TableEntryReference.Key);
        }

        var noButtonShow =
            noLocalization.TableReference.ReferenceType != TableReference.Type.Empty &&
            noLocalization.TableEntryReference.ReferenceType != TableEntryReference.Type.Empty;
        dialog.ButtonShow(UIDialog.Buttons.No, noButtonShow);
        if (noButtonShow)
        {
            dialog.ButtonSetLocalization(UIDialog.Buttons.No, noLocalization.TableReference.TableCollectionName, noLocalization.TableEntryReference.Key);
        }

        var cancelButtonShow = 
            cancelLocalization.TableReference.ReferenceType != TableReference.Type.Empty &&
            cancelLocalization.TableEntryReference.ReferenceType != TableEntryReference.Type.Empty;
        dialog.ButtonShow(UIDialog.Buttons.Cancel, cancelButtonShow);
        if (cancelButtonShow)
        {
            dialog.ButtonSetLocalization(UIDialog.Buttons.Cancel, cancelLocalization.TableReference.TableCollectionName, cancelLocalization.TableEntryReference.Key);
        }
    }
}
