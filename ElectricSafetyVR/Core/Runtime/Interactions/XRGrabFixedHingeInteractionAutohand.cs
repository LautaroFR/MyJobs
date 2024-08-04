namespace Assets._ISVR.Core.Runtime.Interactions
{
    using Assets._VRN.Core.Runtime.Interactions;
    using Autohand;
    using UnityEngine;

    public class XRGrabFixedHingeInteractionAutohand : BaseInteraction
    {
        [SerializeField]
        private PhysicsGadgetLever interactable;

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

                interactable.GetComponentInChildren<Grabbable>().onGrab.AddListener(Grab);
                interactable.GetComponentInChildren<Grabbable>().onRelease.AddListener(Drop);
                this.interactable.OnMin.AddListener(HandleInteractableMinLimitReached);
                this.interactable.OnMax.AddListener(HandleInteractableMaxLimitReached);
            }
        }

        protected void OnDisable()
        {
            if (this.interactable != null)
            {
                this.interactable.enabled = false;

                this.interactable.GetComponentInChildren<Grabbable>().onGrab.RemoveListener(Grab);
                this.interactable.GetComponentInChildren<Grabbable>().onRelease.RemoveListener(Drop);
                this.interactable.OnMin.RemoveListener(this.HandleInteractableMinLimitReached);
                this.interactable.OnMax.RemoveListener(this.HandleInteractableMaxLimitReached);
            }
        }

        private void HandleInteractableMaxLimitReached()
        {
            if (this.targetLimit == LimitEnum.Max)
            {
                this.OnInteractionFinished();
            }

        }

        private void HandleInteractableMinLimitReached()
        {
            if (this.targetLimit == LimitEnum.Min)
            {
                this.OnInteractionFinished();
            }
        }

        private void Drop(Hand arg0, Grabbable arg1)
        {
            if (((this.targetLimit == LimitEnum.Min && !this.interactable.isMin)
                               || (this.targetLimit == LimitEnum.Max && !this.interactable.isMax)))
            {
                this.Stop();
            }
        }

        private void Grab(Hand arg0, Grabbable arg1)
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
