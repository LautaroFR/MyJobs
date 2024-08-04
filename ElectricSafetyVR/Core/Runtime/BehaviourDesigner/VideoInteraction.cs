using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;

[TaskCategory("ISVR")]
public class VideoInteraction : Action
{
    public SharedGameObject targetGameObject;
    public bool WaitForCompletion;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [Header("DEPRECATED")]
    public VRN.GOIDs.BehaviorDesigner.SharedGOIDAsset GoidAsset;
#endif

    private VideoInteractionManager videoInteraction;
    private GameObject prevGameObject;

    public override void OnStart()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
        {
            videoInteraction = currentGameObject.GetComponent<VideoInteractionManager>();
            prevGameObject = currentGameObject;
        }

        if (this.videoInteraction != null)
        {
            this.videoInteraction.Play();
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (this.videoInteraction == null)
        {
            return TaskStatus.Failure;
        }

        if (!this.WaitForCompletion || this.videoInteraction.State == VideoInteractionManager.StateEnum.Continue)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}

