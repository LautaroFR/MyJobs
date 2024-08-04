namespace Assets._ISVR.Core.Runtime.Interactions
{
    using System.Collections;

    using Assets._VRN.Core.Runtime.Interactions;
    using Autohand;
    using UnityEngine;

    [DefaultExecutionOrder(100)]
    public class XROnlyGrabInteraction : BaseInteraction
    {
        [SerializeField]
        private Grabbable interactable;

        [SerializeField]
        private bool canDisableGrabbable;

        protected void OnEnable()
        {
            if (this.interactable != null)
            {
                this.interactable.enabled = true;
                this.interactable.onGrab.AddListener(this.HandleInteractableSelectEntered);
            }
        }

        protected void OnDisable()
        {
            if (this.interactable != null)
            {
                if (this.canDisableGrabbable)
                {
                    this.interactable.enabled = false;
                }

                this.interactable.onGrab.RemoveListener(this.HandleInteractableSelectEntered);
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
            this.StartCoroutine(this.WaitAndFinishInteraction(10));
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
