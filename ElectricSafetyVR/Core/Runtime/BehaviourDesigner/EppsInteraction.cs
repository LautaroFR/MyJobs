using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.XR.Interaction.Toolkit;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

[TaskCategory("ISVR")]

public class EppsInteraction : Action
{
    [Tooltip("Allow interaction with epps in grid and manniquin")]
    public SharedBool active;

    private bool _completed;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset element;
#endif

    public override void OnStart()
    {
        foreach (var epp in EPPItem.instances)
        {
            epp.GetComponent<XRGrabInteractable>().enabled = active.Value;
        }

        if (EPPItem.instances == null)
        {
            Debug.LogError("[EPPs] EPP List not found");
        }

        _completed = true;
    }

    public override TaskStatus OnUpdate()
    {
        if (_completed)
            return TaskStatus.Success;

        else return TaskStatus.Running;
    }
}
