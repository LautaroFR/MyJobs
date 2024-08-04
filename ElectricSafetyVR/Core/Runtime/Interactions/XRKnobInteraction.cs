namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Assets._VRN.Core.Runtime.Interactions;

    using UnityEngine;
    using UnityEngine.XR.Content.Interaction;
    using UnityEngine.XR.Interaction.Toolkit;

    public class XRKnobInteraction : BaseInteraction
    {
        [SerializeField]
        private XRKnob interactable;

        [SerializeField]
        private float targetValue;

        [SerializeField]
        [Min(0)]
        private float threshold = 0.1f;

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
                this.interactable.onValueChange.AddListener(this.HandleInteractableValueChanged);
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
                this.interactable.onValueChange.RemoveListener(this.HandleInteractableValueChanged);
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
            if (Mathf.Abs(this.interactable.value - this.targetValue) > this.threshold)
            {
                this.Stop();
            }
        }

        private void HandleInteractableValueChanged(float value)
        {
            if (Mathf.Abs(value - this.targetValue) <= this.threshold)
            {
                this.OnInteractionFinished();
            }
        }
    }
}
