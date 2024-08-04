namespace Assets._ISVR.Core.Runtime.Interactions
{
    using System.Collections;

    using Assets._VRN.Core.Runtime.Interactions;
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    public class XRPokeInteraction : BaseInteraction
    {
        [SerializeField]
        private XRSimpleInteractable interactable;

        [SerializeField]
        private bool finishOnExit;

        protected void OnEnable()
        {
            if (this.interactable != null)
            {
                this.interactable.enabled = true;

                this.interactable.hoverEntered.AddListener(this.HandleInteractableHoverEntered);
                this.interactable.hoverExited.AddListener(this.HandlInteractableHoverExited);
            }
        }


        protected void OnDisable()
        {
            if (this.interactable != null)
            {
                this.interactable.enabled = false;

                this.interactable.hoverEntered.RemoveListener(this.HandleInteractableHoverEntered);
                this.interactable.hoverExited.RemoveListener(this.HandlInteractableHoverExited);
            }
        }

        protected override void PlayInteraction()
        {
        }

        protected override void StopInteraction()
        {
        }

        private void HandleInteractableHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
            this.Play();

            if (!this.finishOnExit)
            {
                this.StartCoroutine(this.WaitAndFinishInteraction(10));
            }
        }

        private void HandlInteractableHoverExited(HoverExitEventArgs hoverExitEventArgs)
        {
            if (this.finishOnExit)
            {
                this.OnInteractionFinished();
            }
        }

        private IEnumerator WaitAndFinishInteraction(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }

            this.OnInteractionFinished();
        }
    }
}
