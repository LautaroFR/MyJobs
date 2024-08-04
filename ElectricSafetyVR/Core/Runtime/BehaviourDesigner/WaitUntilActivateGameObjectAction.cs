using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using BehaviorDesigner.Runtime;

[TaskCategory("ISVR")]
[TaskDescription("Returns Success if the GameObject is active in the hierarchy, otherwise Running.")]
public class WaitUntilActivateGameObjectAction : Action
{
    [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
    public SharedGameObject targetGameObject;

    [Tooltip("Wait until GameObject is active in hierarchy, if its false: wait until GameObject is inactive")]
    public bool waitUntilActivate;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset goidAsset;
#endif

    public override TaskStatus OnUpdate()
    {
        if(targetGameObject.Value == null)
        {
            Debug.LogError("[GameObject] Not found");
            return TaskStatus.Failure;
        }

        if(!waitUntilActivate)
        {
            return !targetGameObject.Value.activeInHierarchy ? TaskStatus.Success : TaskStatus.Running;
        }
        
        return targetGameObject.Value.activeInHierarchy ? TaskStatus.Success : TaskStatus.Running;
    }

    public override void OnReset()
    {
        targetGameObject.Value = null;
    }
}
