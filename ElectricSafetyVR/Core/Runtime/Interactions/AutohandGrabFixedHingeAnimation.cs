namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Autohand;
    using UnityEngine;

    public class AutohandGrabFixedHingeAnimation : MonoBehaviour
    {
        [SerializeField]
        private Transform targetTransform;

        [SerializeField]
        private float startAngleValue;

        [SerializeField]
        private float endAngleValue;

        private PhysicsGadgetLever physicsGadgetLever;

        protected void Awake()
        {
            this.physicsGadgetLever = this.GetComponent<PhysicsGadgetLever>();
        }

        protected void LateUpdate()
        {
            if (this.targetTransform != null)
            {
                var currentAngle = physicsGadgetLever.GetValue();
                var normalizedAngle = (currentAngle / 2) + 0.5f;
                var eulerAngles = this.targetTransform.eulerAngles;
                eulerAngles.z = Mathf.LerpAngle(
                    this.startAngleValue,
                    this.endAngleValue,
                    normalizedAngle);

                this.targetTransform.eulerAngles = eulerAngles;
            }
        }
    }
}
