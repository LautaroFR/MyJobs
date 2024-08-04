namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Assets._VRN.Core.Runtime.Interactions;
    using Autohand;
    using UnityEngine;

    public class XRGrabInteraction : BaseInteraction
    {
        [SerializeField]
        private Grabbable interactable;

        [SerializeField]
        private ItemPositioner itemPositioner;

        protected void OnEnable()
        {
            if (this.interactable != null)
            {
                this.interactable.enabled = true;
                foreach (var interactableCollider in this.interactable.grabColliders)
                {
                    interactableCollider.enabled = true;
                }

                this.interactable.onGrab.AddListener(this.HandleInteractableSelectEntered);
                this.interactable.onRelease.AddListener(this.HandleInteractableSelectExited);
            }

            if (this.itemPositioner != null)
            {
                this.itemPositioner.enabled = true;
                this.itemPositioner.OnPosition.AddListener(this.HandleInteractablePlaced);
            }
        }

        protected void OnDisable()
        {
            if (this.interactable != null)
            {
                this.interactable.enabled = false;
                foreach (var interactableCollider in this.interactable.grabColliders)
                {
                    interactableCollider.enabled = false;
                }

                this.interactable.onGrab.RemoveListener(this.HandleInteractableSelectEntered);
                this.interactable.onRelease.RemoveListener(this.HandleInteractableSelectExited);
            }

            if (this.itemPositioner != null)
            {
                this.itemPositioner.enabled = false;
                this.itemPositioner.OnPosition.RemoveListener(this.HandleInteractablePlaced);
            }
        }

        protected override void PlayInteraction()
        {
        }

        protected override void StopInteraction()
        {
        }

        private void HandleInteractableSelectEntered(Hand arg0, Grabbable arg1)
        {
            this.Play();
        }

        private void HandleInteractableSelectExited(Hand arg0, Grabbable arg1)
        {
            if (this.itemPositioner == null)
                return;

            if (!this.itemPositioner.ItemInZone)
            {
                this.Stop();
            }
        }

        private void HandleInteractablePlaced(ItemPositioner arg)
        {
            this.OnInteractionFinished();
        }
    }
}
