namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Assets._VRN.Core.Runtime.Interactions;
    using Assets._VRN.Core.Runtime.XR.Interactables;

    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    public class XRGrabHingeInteraction : BaseInteraction
    {
        [SerializeField]
        private XRGrabHingeInteractable interactable;

        [SerializeField]
        private float targetAngle;

        [SerializeField]
        [Min(0)]
        private float threshold = 5f;

        protected void OnEnable()
        {
            if (this.interactable != null)
            {
                this.interactable.enabled = true;
                foreach (var interactableCollider in this.interactable.colliders)
                {
                    interactableCollider.enabled = true;
                }

                this.interactable.selectEntered.AddListener(this.HandleInteractableSelectEntered);
                this.interactable.selectExited.AddListener(this.HandleInteractableSelectExited);
                this.interactable.OnAngleChanged.AddListener(this.HandleInteractableAngleValueChanged);
            }
        }

        protected void OnDisable()
        {
            if (this.interactable != null)
            {
                this.interactable.enabled = false;
                foreach (var interactableCollider in this.interactable.colliders)
                {
                    interactableCollider.enabled = false;
                }

                this.interactable.selectEntered.RemoveListener(this.HandleInteractableSelectEntered);
                this.interactable.selectExited.RemoveListener(this.HandleInteractableSelectExited);
                this.interactable.OnAngleChanged.RemoveListener(this.HandleInteractableAngleValueChanged);
            }
        }

        protected override void PlayInteraction()
        {
        }

        protected override void StopInteraction()
        {
        }

        private void HandleInteractableSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
            this.Play();
        }

        private void HandleInteractableSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
            if (Mathf.Abs(this.interactable.CurrentAngle - this.targetAngle) > this.threshold)
            {
                this.Stop();
            }
        }

        private void HandleInteractableAngleValueChanged(float value)
        {
            if (Mathf.Abs(value - this.targetAngle) <= this.threshold)
            {
                this.OnInteractionFinished();
            }
        }
    }
}
