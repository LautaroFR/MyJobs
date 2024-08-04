using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;

[TaskCategory("ISVR")]
public class FireInteraction : Action
{
    public SharedGameObject targetGameObject;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset element;
#endif

    private FireManager _fire;

    private bool _onBurnedTriggered;
    private bool _onFireControlledTriggered;
    

    public override void OnStart()
    {
        if (targetGameObject.Value != null)
        {
            _fire = targetGameObject.Value.GetComponent<FireManager>();
        }

        if (_fire == null)
        {
            Debug.LogError("[FireManager] Not found");
        }
        else
        {
            _fire.OnBurned.AddListener(OnBurnedAction);
            _fire.OnFireControlled.AddListener(OnFireControlledAction);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_fire == null)
        {
            Debug.LogError("[VariableWatcher] Not found");
            return TaskStatus.Failure;
        }

        if (_onBurnedTriggered)
            return TaskStatus.Failure;

        if (_onFireControlledTriggered)
            return TaskStatus.Success;

        return TaskStatus.Running;
    }

    private void OnFireControlledAction(FireManager arg0)
    {
        _onFireControlledTriggered = true;
    }

    private void OnBurnedAction(FireManager arg0)
    {
        _onBurnedTriggered = true;
    }
}
