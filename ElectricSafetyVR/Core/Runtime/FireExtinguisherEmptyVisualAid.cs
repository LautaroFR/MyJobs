namespace Assets._ISVR.Core.Runtime
{
    using Assets._VRN.Core.Runtime.Highlight;

    using UnityEngine;

    [RequireComponent(typeof(OutlineHighlight))]
    public class FireExtinguisherEmptyVisualAid : MonoBehaviour
    {
        [SerializeField]
        private FireExtinguisherController fireExtinguisherController;

        private OutlineHighlight outlineHighlight;

        protected void Awake()
        {
            this.outlineHighlight = this.GetComponent<OutlineHighlight>();
        }

        protected void OnEnable()
        {
            if (this.fireExtinguisherController != null)
            {
                this.fireExtinguisherController.OnStartEmpty.AddListener(this.HandleStartEmpty);
                this.fireExtinguisherController.OnStopEmpty.AddListener(this.HandleStopEmpty);
            }
        }

        protected void OnDisable()
        {
            if (this.fireExtinguisherController != null)
            {
                this.fireExtinguisherController.OnStartEmpty.RemoveListener(this.HandleStartEmpty);
                this.fireExtinguisherController.OnStopEmpty.RemoveListener(this.HandleStopEmpty);
            }
        }

        private void HandleStartEmpty(FireExtinguisherController arg0)
        {
            this.outlineHighlight.StartHighlight();
        }

        private void HandleStopEmpty(FireExtinguisherController arg0)
        {
            this.outlineHighlight.StopHighlight();
        }
    }
}
