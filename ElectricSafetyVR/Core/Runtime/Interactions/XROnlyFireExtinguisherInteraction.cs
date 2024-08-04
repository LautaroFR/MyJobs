namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Assets._VRN.Core.Runtime.Interactions;
    using UnityEngine;

    public class XROnlyFireExtinguisherInteraction : BaseInteraction
    {
        [SerializeField]
        private FireExtinguisherController fireExtinguisher;

        protected void OnEnable()
        {
            if (this.fireExtinguisher != null)
            {
                this.fireExtinguisher.OnStartApplying.AddListener(this.HandleStartApplying);
                this.fireExtinguisher.OnStopApplying.AddListener(this.HandleStopApplying);
            }
        }

        protected void OnDisable()
        {
            if (this.fireExtinguisher != null)
            {
                this.fireExtinguisher.OnStartApplying.RemoveListener(this.HandleStartApplying);
                this.fireExtinguisher.OnStopApplying.RemoveListener(this.HandleStopApplying);
            }
        }

        private void HandleStartApplying(FireExtinguisherController arg0)
        {
            this.Play();
        }

        private void HandleStopApplying(FireExtinguisherController arg0)
        {
            this.Stop();
        }
        
        protected override void PlayInteraction()
        {
        }

        protected override void StopInteraction()
        {
        }
    }
}