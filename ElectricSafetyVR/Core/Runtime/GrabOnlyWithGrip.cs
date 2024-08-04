using UnityEngine;
using UnityEngine.InputSystem;

public class GrabOnlyWithGrip : MonoBehaviour
{    
    [SerializeField]
    private InputActionReference rightHandSelectReference;
    [SerializeField]
    private InputActionReference leftHandSelectReference;

    private void Start()
    {
        rightHandSelectReference.action.ChangeBindingWithPath("<XRController>{RightHand}/triggerPressed").Erase();
        leftHandSelectReference.action.ChangeBindingWithPath("<XRController>{LeftHand}/triggerPressed").Erase();
    }

    private void OnDisable()
    {
        rightHandSelectReference.action.AddBinding("<XRController>{RightHand}/triggerPressed");
        leftHandSelectReference.action.AddBinding("<XRController>{LeftHand}/triggerPressed");
    }
}
