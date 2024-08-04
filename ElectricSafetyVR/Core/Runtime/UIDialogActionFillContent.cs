using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("ISVR/(ISVR) UI Dialog")]
[TaskName("Fill Content")]
public class UIDialogActionFillContent : Action
{
    public override string FriendlyName { get => string.IsNullOrEmpty(base.FriendlyName) ? "Fill Content" : base.FriendlyName; set => base.FriendlyName = value; }

    public SharedGameObject dialog;

    public SharedObject preset;

    [Space]
    public SharedString yesTable;
    public SharedString yesEntry;

    [Space]
    public SharedString noTable;
    public SharedString noEntry;

    [Space]
    public SharedString cancelTable;
    public SharedString cancelEntry;

    public override void OnStart()
    {
        var dialog = this.dialog.Value.GetComponent<UIDialog>();

        if (preset.Value != null)
        {
            var preset = this.preset.Value as UIDialogPreset;
            preset.ApplyPreset(dialog);
            return;
        }

        var show = string.IsNullOrEmpty(yesTable.Value) == false && string.IsNullOrEmpty(yesEntry.Value) == false;
        dialog.ButtonShow(UIDialog.Buttons.Yes, show);
        if (show)
        {
            dialog.ButtonSetLocalization(UIDialog.Buttons.Yes, yesTable.Value, yesEntry.Value);
        }

        show = string.IsNullOrEmpty(noTable.Value) == false && string.IsNullOrEmpty(noEntry.Value) == false;
        dialog.ButtonShow(UIDialog.Buttons.No, show);
        if (show)
        {
            dialog.ButtonSetLocalization(UIDialog.Buttons.No, noTable.Value, noEntry.Value);
        }

        show = string.IsNullOrEmpty(cancelTable.Value) == false && string.IsNullOrEmpty(cancelEntry.Value) == false;
        dialog.ButtonShow(UIDialog.Buttons.Cancel, show);
        if (show)
        {
            dialog.ButtonSetLocalization(UIDialog.Buttons.Cancel, cancelTable.Value, cancelEntry.Value);
        }
    }
}
