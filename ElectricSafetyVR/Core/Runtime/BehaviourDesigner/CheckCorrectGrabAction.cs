using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.XR.Interaction.Toolkit;

[TaskCategory("ISVR")]
public class CheckCorrectGrabAction : Action
{
    public SharedGameObject targetGameObject;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset element;
#endif

    private CheckIfElementIsPositioned _variableWatcher;

    public override void OnStart()
    {
        if (targetGameObject.Value != null)
        {
            _variableWatcher = targetGameObject.Value.GetComponent<CheckIfElementIsPositioned>();
        }

        if (_variableWatcher == null)
        {
            Debug.LogError("[VariableWatcher] Not found");
        }
        else
        {
            _variableWatcher.VariableChanged += OnVariableChanged;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if(_variableWatcher == null)
        {
            Debug.LogError("[VariableWatcher] Not found");
            return TaskStatus.Failure;
        }

        if (_variableWatcher.IsInPosition)
        {
            Debug.Log("El objeto ha entrado en la bandeja.");
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void OnVariableChanged() {
    }
}
