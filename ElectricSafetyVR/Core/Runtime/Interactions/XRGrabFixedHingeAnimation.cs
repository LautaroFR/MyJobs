namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Assets._VRN.Core.Runtime.XR.Interactables;

    using UnityEngine;

    [RequireComponent(typeof(XRGrabFixedHingeInteractable))]
    public class XRGrabFixedHingeAnimation : MonoBehaviour
    {
        [SerializeField]
        private Transform targetTransform;

        [SerializeField]
        private float startAngleValue;

        [SerializeField]
        private float endAngleValue;

        private XRGrabFixedHingeInteractable grabFixedHingeInteractable;

        protected void Awake()
        {
            this.grabFixedHingeInteractable = this.GetComponent<XRGrabFixedHingeInteractable>();
        }

        protected void LateUpdate()
        {
            if (this.targetTransform != null)
            {
                var eulerAngles = this.targetTransform.eulerAngles;
                eulerAngles.z = Mathf.LerpAngle(
                    this.startAngleValue,
                    this.endAngleValue,
                    this.grabFixedHingeInteractable.CurrentNormalizedAngle);

                this.targetTransform.eulerAngles = eulerAngles;
            }
        }
    }
}
