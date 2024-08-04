namespace Assets._ISVR.Core.Runtime.Interactions
{
    using System.Collections;
    using System.Collections.Generic;

    using Assets._VRN.Core.Runtime.Interactions;

    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    public class XRTeleportInteraction : BaseInteraction
    {
        [SerializeField]
        private List<TeleportationArea> teleportationAreas = new List<TeleportationArea>();

        protected void OnEnable()
        {
            foreach (var teleportationArea in this.teleportationAreas)
            {
                teleportationArea.teleporting.AddListener(this.HandleTeleporting);
            }
        }

        protected void OnDisable()
        {
            foreach (var teleportationArea in this.teleportationAreas)
            {
                teleportationArea.teleporting.RemoveListener(this.HandleTeleporting);
            }
        }

        protected override void PlayInteraction()
        {
        }

        protected override void StopInteraction()
        {
        }

        private void HandleTeleporting(TeleportingEventArgs teleportingEventArgs)
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
