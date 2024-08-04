using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[TaskCategory("ISVR.Controllers")]
[TaskDescription("Return success when you press one of the input action from the list")]
// NOTE: name is misleading, should be called something like IsPressed and category InputAction
public class CheckControlButtonPressAction : Action
{
    // NOTE: if this at some needs to be passed a shared variables it could be changed to SharedObjectList
    [SerializeField]
    private List<InputActionReference> inputActionList = new List<InputActionReference>();

    public override TaskStatus OnUpdate()
    {
        foreach (InputAction button in inputActionList)
        {
            if (button.IsPressed())
            {
                return TaskStatus.Success;
            }
        }
        return TaskStatus.Running;        
    }
}