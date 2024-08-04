namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Assets._VRN.Core.Runtime.Interactions;
    using UnityEngine;
    public class XRFireExtinguisherInteraction : BaseInteraction
    {
        [SerializeField]
        private TargetController target;

        [SerializeField]
        private FireExtinguisherController fireExtinguisher;

        protected void OnEnable()
        {
            if (this.target != null)
            {
                this.fireExtinguisher.OnStopApplying.AddListener(this.HandleInteractableStopApplying);
                this.target.OnApplying.AddListener(this.HandleInteractableApplying);
                this.target.OnReachObjetive.AddListener(this.HandleInteractableReachObjetive);
            }
        }

        protected void OnDisable()
        {
            if (this.target != null)
            {
                this.fireExtinguisher.OnStopApplying.RemoveListener(this.HandleInteractableStopApplying);
                this.target.OnApplying.RemoveListener(this.HandleInteractableApplying);
                this.target.OnReachObjetive.RemoveListener(this.HandleInteractableReachObjetive);
            }
        }

        private void HandleInteractableReachObjetive(TargetController arg0)
        {
            this.OnInteractionFinished();
        }

        private void HandleInteractableStopApplying(FireExtinguisherController arg0)
        {
            this.Stop();
        }

        private void HandleInteractableApplying(TargetController arg0)
        {
            this.Play();
        }

        protected override void PlayInteraction()
        {
        }

        protected override void StopInteraction()
        {
        }
    }
}