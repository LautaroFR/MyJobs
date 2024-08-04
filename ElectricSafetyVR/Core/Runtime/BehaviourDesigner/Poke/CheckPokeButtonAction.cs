using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using BehaviorDesigner.Runtime;

[TaskCategory("ISVR.Poke")]
public class CheckPokeButtonAction : Action
{
    public SharedGameObject targetGameObject;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset ButtonInteractable;
#endif

    private bool _isPressed;
    private XRSimpleInteractable _button;

    public override void OnStart()
    {
        _isPressed = false;

        if (targetGameObject.Value != null)
        {
            _button = targetGameObject.Value.GetComponent<XRSimpleInteractable>();
        }

        if (_button != null)
        {
            _button.hoverEntered.AddListener(OnPress);
        }
    }

    private void OnPress(HoverEnterEventArgs arg0) => _isPressed = true;

    public override TaskStatus OnUpdate()
    {
        if (_button == null)
        {
            Debug.Log("[PokeButton] Not assigned or incorrect type");
            return TaskStatus.Failure;
        }

        if (_isPressed)
        {
            RemoveListener();
            return TaskStatus.Success;
        }

        else
        {
            return TaskStatus.Running;
        }
    }

    private void RemoveListener() => _button.hoverEntered.RemoveListener(OnPress);
}
