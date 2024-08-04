namespace Assets._ISVR.Core.Runtime
{
    using System.Collections;

    using Assets._VRN.Core.Runtime.Interactions;

    using UnityEngine;

    public class TransformAnimation : BaseInteraction
    {
        [SerializeField]
        private Transform workingTransform;

        [SerializeField]
        private Transform targetTransform;

        [SerializeField]
        [Min(0)]
        private float duration = 1f;

        protected override void PlayInteraction()
        {
            this.StartCoroutine(this.Animate());
        }

        protected override void StopInteraction()
        {
            this.StopAllCoroutines();
        }

        private IEnumerator Animate()
        {
            var currentTime = 0f;

            var initialPosition = this.workingTransform.position;
            var initialRotation = this.workingTransform.rotation;
            while (currentTime < this.duration)
            {
                var normalizedTime = Mathf.InverseLerp(0, this.duration, currentTime);

                this.workingTransform.SetPositionAndRotation(
                    Vector3.Slerp(initialPosition, this.targetTransform.position, normalizedTime),
                    Quaternion.Slerp(initialRotation, this.targetTransform.rotation, normalizedTime));

                yield return null;

                currentTime += Time.deltaTime;
            }

            this.workingTransform.SetPositionAndRotation(this.targetTransform.position, this.targetTransform.rotation);
            this.OnInteractionFinished();
        }
    }
}
