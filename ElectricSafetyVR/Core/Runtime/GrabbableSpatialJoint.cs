namespace Assets._ISVR.Core.Runtime
{
    using System;
    using System.Collections;

    using Autohand;

    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Grabbable))]
    public class GrabbableSpatialJoint : MonoBehaviour
    {
        [SerializeField]
        private Vector3 anchor;

        [Space]

        [SerializeField]
        private Rigidbody connectedBody;

        [SerializeField]
        private Vector3 connectedAnchor;

        [Space]

        [SerializeField]
        [Min(0)]
        private float nearDistance;

        [SerializeField]
        [Min(0)]
        private float farDistance;

        [Space]

        [SerializeField]
        private AngularLimit angularXLimit;

        [SerializeField]
        private AngularLimit angularYLimit;

        [SerializeField]
        private AngularLimit angularZLimit;

        private Rigidbody rigidbody;

        private Grabbable grabbable;

        private Vector3 restPosition;

        private Quaternion restRotation;

        private Vector3 forwardAxis;

        private Vector3 upwardAxis;

        private float distance;

        private float angularX;

        private float angularY;

        private float angularZ;

        public Vector3 WorldAnchor => this.transform.TransformPoint(this.anchor);

        public Vector3 ConnectedWorldAnchor =>
            this.connectedBody != null
                ? this.connectedBody.transform.TransformPoint(this.connectedAnchor)
                : this.connectedAnchor;

        public Quaternion WorldRotation
        {
            get
            {
                if (Application.isPlaying)
                {
                    if (this.connectedBody != null)
                    {
                        return Quaternion.LookRotation(
                            this.connectedBody.transform.TransformVector(this.forwardAxis),
                            this.connectedBody.transform.TransformVector(this.upwardAxis));
                    }

                    return Quaternion.LookRotation(this.forwardAxis, this.upwardAxis);
                }

                return Quaternion.LookRotation(this.WorldAnchor - this.ConnectedWorldAnchor, this.transform.up);
            }
        }

        public Vector3 WorldForwardAxis => this.WorldRotation * Vector3.forward;

        public Vector3 WorldRightAxis => this.WorldRotation * Vector3.right;

        public Vector3 WorldUpAxis => this.WorldRotation * Vector3.up;

        protected void Awake()
        {
            this.grabbable = this.GetComponent<Grabbable>();
            this.rigidbody = this.GetComponent<Rigidbody>();

            this.rigidbody.isKinematic = true;

            this.restPosition = this.connectedBody != null
                                    ? this.connectedBody.transform.InverseTransformPoint(this.transform.position)
                                    : this.transform.position;

            this.restRotation = this.connectedBody != null
                                    ? Quaternion.Inverse(this.connectedBody.transform.rotation)
                                      * this.transform.rotation
                                    : this.transform.rotation;

            this.forwardAxis = this.WorldAnchor - this.ConnectedWorldAnchor;
            this.upwardAxis = this.transform.up;

            if (this.connectedBody != null)
            {
                foreach (var bodyCollider in this.connectedBody.GetComponentsInChildren<Collider>(true))
                {
                    foreach (var myCollider in this.GetComponentsInChildren<Collider>(true))
                    {
                        Physics.IgnoreCollision(bodyCollider, myCollider, true);
                    }
                }

                this.forwardAxis = this.connectedBody.transform.InverseTransformVector(this.forwardAxis);
                this.upwardAxis = this.connectedBody.transform.InverseTransformVector(this.upwardAxis);
            }
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

        protected void FixedUpdate()
        {
            if (this.rigidbody.isKinematic)
            {
                this.transform.SetPositionAndRotation(
                    this.connectedBody != null
                        ? this.connectedBody.transform.TransformPoint(this.restPosition)
                        : this.restPosition,
                    this.connectedBody != null ? this.connectedBody.transform.rotation * this.restRotation : this.restRotation);
            }
        }

        protected void OnValidate()
        {
            this.farDistance = Mathf.Max(this.nearDistance, this.farDistance);
            this.angularXLimit.Maximum = Mathf.Max(this.angularXLimit.Minimum, this.angularXLimit.Maximum);
            this.angularYLimit.Maximum = Mathf.Max(this.angularYLimit.Minimum, this.angularYLimit.Maximum);
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.SphereHandleCap(
                0,
                this.WorldAnchor,
                Quaternion.identity,
                0.01f,
                EventType.Repaint);


            var connectedWorldAnchor = this.ConnectedWorldAnchor;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.SphereHandleCap(
                0,
                connectedWorldAnchor,
                Quaternion.identity,
                0.01f,
                EventType.Repaint);

            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.DrawLine(this.WorldAnchor, this.ConnectedWorldAnchor);

            UnityEditor.Handles.RadiusHandle(Quaternion.identity, connectedWorldAnchor, this.nearDistance);
            UnityEditor.Handles.RadiusHandle(Quaternion.identity, connectedWorldAnchor, this.farDistance);

            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.ArrowHandleCap(
                0,
                connectedWorldAnchor,
                this.WorldRotation,
                UnityEditor.HandleUtility.GetHandleSize(connectedWorldAnchor),
                EventType.Repaint);

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.ArrowHandleCap(
                0,
                connectedWorldAnchor,
                Quaternion.LookRotation(this.WorldRightAxis),
                UnityEditor.HandleUtility.GetHandleSize(connectedWorldAnchor),
                EventType.Repaint);

            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.ArrowHandleCap(
                0,
                connectedWorldAnchor,
                Quaternion.LookRotation(this.WorldUpAxis),
                UnityEditor.HandleUtility.GetHandleSize(connectedWorldAnchor),
                EventType.Repaint);

            UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.1f);
            var angle = this.angularXLimit.Maximum - this.angularXLimit.Minimum;
            var from = Quaternion.AngleAxis(this.angularXLimit.Minimum, this.WorldRightAxis) * this.WorldForwardAxis;
            UnityEditor.Handles.DrawSolidArc(connectedWorldAnchor, this.WorldRightAxis, from, angle, UnityEditor.HandleUtility.GetHandleSize(connectedWorldAnchor));

            UnityEditor.Handles.color = new Color(0f, 1f, 0f, 0.1f);
            angle = this.angularYLimit.Maximum - this.angularYLimit.Minimum;
            from = Quaternion.AngleAxis(this.angularYLimit.Minimum, this.WorldUpAxis) * this.WorldForwardAxis;
            UnityEditor.Handles.DrawSolidArc(connectedWorldAnchor, this.WorldUpAxis, from, angle, UnityEditor.HandleUtility.GetHandleSize(connectedWorldAnchor));

            UnityEditor.Handles.color = new Color(0f, 0f, 1f, 0.1f);
            angle = this.angularZLimit.Maximum - this.angularZLimit.Minimum;
            from = Quaternion.AngleAxis(this.angularZLimit.Minimum, this.WorldForwardAxis) * this.WorldUpAxis;
            UnityEditor.Handles.DrawSolidArc(connectedWorldAnchor, this.WorldForwardAxis, from, angle, UnityEditor.HandleUtility.GetHandleSize(connectedWorldAnchor));
        }

#endif

        private void HandleGrab(Hand hand, Grabbable g)
        {
            this.rigidbody.isKinematic = false;

            this.StopAllCoroutines();
            this.StartCoroutine(this.HandleJoint());
        }

        private void HandleRelease(Hand hand, Grabbable g)
        {
            if (!this.grabbable.IsHeld())
            {
                this.rigidbody.isKinematic = true;
            }

            this.StopAllCoroutines();
        }

        private IEnumerator HandleJoint()
        {
            var plane = new Plane();
            while (true)
            {
                var worldAnchor = this.WorldAnchor;
                var connectedWorldAnchor = this.ConnectedWorldAnchor;

                this.distance = Vector3.Distance(worldAnchor, connectedWorldAnchor);
                if (this.distance < this.nearDistance || this.distance > this.farDistance)
                {
                    this.grabbable.ForceHandsRelease();
                    yield break;
                }

                var worldForwardAxis = this.WorldForwardAxis;
                var worldRightAxis = this.WorldRightAxis;
                var worldUpAxis = this.WorldUpAxis;

                plane.SetNormalAndPosition(worldRightAxis, connectedWorldAnchor);
                this.angularX = Mathf.DeltaAngle(
                    Vector3.SignedAngle(
                        (plane.ClosestPointOnPlane(worldAnchor) - connectedWorldAnchor).normalized,
                        worldForwardAxis,
                        worldRightAxis),
                    0);

                if (this.angularX < this.angularXLimit.Minimum || this.angularX > this.angularXLimit.Maximum)
                {
                    this.grabbable.ForceHandsRelease();
                    yield break;
                }

                plane.SetNormalAndPosition(worldUpAxis, connectedWorldAnchor);
                this.angularY = Mathf.DeltaAngle(
                    Vector3.SignedAngle(
                        (plane.ClosestPointOnPlane(worldAnchor) - connectedWorldAnchor).normalized,
                        worldForwardAxis,
                        worldUpAxis),
                    0);

                if (this.angularY < this.angularYLimit.Minimum || this.angularY > this.angularYLimit.Maximum)
                {
                    this.grabbable.ForceHandsRelease();
                    yield break;
                }

                plane.SetNormalAndPosition(worldForwardAxis, connectedWorldAnchor);
                this.angularZ = Mathf.DeltaAngle(
                    Vector3.SignedAngle(
                        (plane.ClosestPointOnPlane(worldAnchor) - connectedWorldAnchor).normalized,
                        worldUpAxis,
                        worldForwardAxis),
                    0);

                if (this.angularZ < this.angularZLimit.Minimum || this.angularZ > this.angularZLimit.Maximum)
                {
                    this.grabbable.ForceHandsRelease();
                    yield break;
                }

                yield return null;
            }
        }

        [Serializable]
        public struct AngularLimit
        {
            public float Minimum;

            public float Maximum;
        }
    }
}
