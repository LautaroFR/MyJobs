using Autohand;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PushButtonHandController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If it is empty, hand will adopt relaxed pose as default")]
    private GrabbablePose handPose;

    private Hand[] _hands;

    private void OnEnable()
    {
        _hands = FindObjectsOfType<Hand>();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<Hand>(out var hand);

        if (hand != null && !hand.grabbing)
        {
            hand.enableIK = false;
            if (handPose == null)
                hand.OpenHand();
            else
                hand.SetHandPose(handPose);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent<Hand>(out var hand);

        if (hand != null && !hand.grabbing)
        {
            hand.enableIK = true;
            hand.RelaxHand();
        }
    }

    private void OnDisable()
    {
        foreach (Hand hand in _hands)
        {
            if (!hand.grabbing)
            {
                hand.enableIK = true;
                hand.RelaxHand();
            }
        }
    }
}
