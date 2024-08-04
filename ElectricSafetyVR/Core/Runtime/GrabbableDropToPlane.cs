namespace Assets._ISVR.Core.Runtime
{
    using System.Collections;

    using Autohand;

    using UnityEngine;

    [RequireComponent(typeof(Grabbable))]
    public class GrabbableDropToPlane : MonoBehaviour
    {
        [SerializeField]
        [Min(0)]
        private float waitToDrop = 1f;

        [SerializeField]
        [Min(0)]
        private float offset = 0f;

        [SerializeField]
        private LayerMask layerMask = Physics.AllLayers;

        [Space]

        [SerializeField]
        [Min(0.1f)]
        private float linearVelocity = 0.5f;

        [SerializeField]
        [Min(1f)]
        private float angularVelocity = 45f;

        private Grabbable grabbable;

        private Rigidbody rigidbody;

        private Bounds bounds;

        private CollisionDetectionMode collisionDetectionMode;

        private bool kinematic;

        private bool moving;

        protected void Awake()
        {
            this.grabbable = this.GetComponent<Grabbable>();
            this.rigidbody = this.GetComponent<Rigidbody>();

            this.BuildBounds();
        }

        protected void OnEnable()
        {
            this.grabbable.onGrab.AddListener(this.HandleGrab);
            this.grabbable.onRelease.AddListener(this.HandleRelease);
        }

        protected void OnDisable()
        {
            this.grabbable.onGrab.RemoveListener(this.HandleGrab);
            this.grabbable.onRelease.RemoveListener(this.HandleRelease);
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
        }

        private void BuildBounds()
        {
            var tempBounds = new Bounds();

            foreach (var colliderComponent in this.GetComponentsInChildren<Collider>())
            {
                if (!colliderComponent.isTrigger && !colliderComponent.bounds.extents.Equals(Vector3.zero))
                {
                    var colliderBounds = new Bounds(
                        this.transform.InverseTransformPoint(colliderComponent.bounds.center),
                        this.transform.InverseTransformVector(colliderComponent.bounds.size));

                    if (tempBounds.extents.Equals(Vector3.zero))
                    {
                        tempBounds = colliderBounds;
                    }
                    else
                    {
                        tempBounds.Encapsulate(colliderBounds);
                    }
                }
            }

            this.bounds = tempBounds;
        }

        private void HandleGrab(Hand hand, Grabbable g)
        {
            this.StopAllCoroutines();
        }

        private void HandleRelease(Hand hand, Grabbable g)
        {
            if (!this.grabbable.IsHeld())
            {
                this.StartCoroutine(this.DropCoroutine());
            }
        }

        private IEnumerator DropCoroutine()
        {
            yield return new WaitForSeconds(this.waitToDrop);

            var pivotOffset = -this.bounds.center;
            pivotOffset.y += this.offset + this.bounds.extents.y;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, this.layerMask, QueryTriggerInteraction.Ignore))
            {
                var targetPosition = hit.point + pivotOffset;
                var targetRotation = Quaternion.Euler(0f, this.transform.eulerAngles.y, 0f);

                yield return this.StartCoroutine(this.MoveTo(targetPosition, targetRotation));
            }
        }

        private IEnumerator MoveTo(Vector3 targetPosition, Quaternion targetRotation)
        {
            this.BeginMove();

            var sourcePosition = this.transform.position;
            var sourceRotation = this.transform.rotation;

            var linearTime = Vector3.Distance(sourcePosition, targetPosition) / this.linearVelocity;
            var angularTime = Quaternion.Angle(sourceRotation, targetRotation) / this.angularVelocity;

            var time = Mathf.Max(linearTime, angularTime);
            var currentTime = 0f;

            while (currentTime < time)
            {
                var t = Mathf.InverseLerp(0, time, currentTime);
                this.transform.SetPositionAndRotation(
                    Vector3.Lerp(sourcePosition, targetPosition, t),
                    Quaternion.Slerp(sourceRotation, targetRotation, t));

                yield return null;

                currentTime += Time.deltaTime;
            }

            this.transform.SetPositionAndRotation(targetPosition, targetRotation);

            this.EndMove();
        }

        private void BeginMove()
        {
            if (!this.moving)
            {
                this.collisionDetectionMode = this.rigidbody.collisionDetectionMode;
                this.kinematic = this.rigidbody.isKinematic;

                this.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                this.rigidbody.isKinematic = true;
            }
        }

        private void EndMove()
        {
            if (this.moving)
            {
                this.rigidbody.collisionDetectionMode = this.collisionDetectionMode;
                this.rigidbody.isKinematic = this.kinematic;

                this.moving = false;
            }
        }
    }
}
