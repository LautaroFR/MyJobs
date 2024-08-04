using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("ISVR.Controllers")]
public class LookAtControllersEvaluate : Action
{
    [Header("Controllers with Skinned Mesh Renderer component")]
    public SharedGameObject leftTargetGameObject;
    public SharedGameObject rightTargetGameObject;


#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset leftController;
    public VRN.GOIDs.GOIDAsset rightController;
#endif

    private SkinnedMeshRenderer _leftController;
    private SkinnedMeshRenderer _rightController;

    public override void OnStart()
    {
        if (leftTargetGameObject.Value != null)
        {
             _leftController  = leftTargetGameObject.Value.GetComponent<SkinnedMeshRenderer>();
        }

        if (rightTargetGameObject.Value != null)
        {
            _rightController = rightTargetGameObject.Value.GetComponent<SkinnedMeshRenderer>();
        }
    }

    public override TaskStatus OnUpdate()
    {
        if(_leftController == null || _rightController == null)
        {
            Debug.LogError("[Controllers] No references found");
            return TaskStatus.Failure;
        }

        if (!_leftController.isVisible || !_rightController.isVisible)
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}