using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using BehaviorDesigner.Runtime;

[TaskCategory("ISVR.Poke")]
public class SetButtonInteractable : Action
{
    public SharedGameObject targetGameObject;
    public bool IsInteractable;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset PokeButton;
#endif

    private XRSimpleInteractable _button;

    public override void OnStart()
    {
        if (targetGameObject.Value != null)
        {
            _button = targetGameObject.Value.GetComponent<XRSimpleInteractable>();
        }

        if (_button != null)
        {
            _button.enabled = IsInteractable;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if(_button == null)
        {
            Debug.Log("[SetButtonInteractable] Not assigned or incorrect type");
            return TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}
