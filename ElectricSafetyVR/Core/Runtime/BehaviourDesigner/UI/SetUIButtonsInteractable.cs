using BehaviorDesigner.Runtime.Tasks;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime;

[TaskCategory("ISVR.UI")]
public class SetUIButtonsInteractable : Action
{
    public SharedGameObject targetGameObject;
    public bool IsInteractable;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset buttonsParent;
#endif

    private GameObject _parent;

    public override void OnStart()
    {
        _parent = targetGameObject.Value;

        if (_parent == null)
        {
            return;
        }

        foreach (Button button in _parent.GetComponentsInChildren<Button>())
        {
            button.interactable = IsInteractable;
        }

        foreach (ButtonManager button in _parent.GetComponentsInChildren<ButtonManager>())
        {
            button.isInteractable = IsInteractable;
            button.UpdateUI();
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_parent == null)
        {
            Debug.Log("[SetUIButtonsInteractable] Not assigned or incorrect type");
            return TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}
