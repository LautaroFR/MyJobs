namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Assets._VRN.Core.Runtime.Interactions;
    using Assets._VRN.Core.Runtime.XR.Interactables;

    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    public class XRGrabFixedHingeInteraction : BaseInteraction
    {
        [SerializeField]
        private XRGrabFixedHingeInteractable interactable;

        [SerializeField]
        private LimitEnum targetLimit;

        public enum LimitEnum
        {
            Min,
            Max
        }

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
                this.interactable.OnMinLimitReached.AddListener(this.HandleInteractableMinLimitReached);
                this.interactable.OnMaxLimitReached.AddListener(this.HandleInteractableMaxLimitReached);
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
                this.interactable.OnMinLimitReached.RemoveListener(this.HandleInteractableMinLimitReached);
                this.interactable.OnMaxLimitReached.RemoveListener(this.HandleInteractableMaxLimitReached);
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
            if (!this.interactable.IsAutoSnapping
                && ((this.targetLimit == LimitEnum.Min && !this.interactable.IsMinLimitReached)
                    || (this.targetLimit == LimitEnum.Max && !this.interactable.IsMaxLimitReached)))
            {
                this.Stop();
            }
        }

        private void HandleInteractableMinLimitReached(float value)
        {
            if (this.targetLimit == LimitEnum.Min)
            {
                this.OnInteractionFinished();
            }
        }
        
        private void HandleInteractableMaxLimitReached(float value)
        {
            if (this.targetLimit == LimitEnum.Max)
            {
                this.OnInteractionFinished();
            }
        }
    }
}
