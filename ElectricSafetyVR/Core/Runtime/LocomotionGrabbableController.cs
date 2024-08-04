namespace Assets._ISVR.Core.Runtime
{
    using System.Collections.Generic;

    using Assets._VRN.Core.Runtime.XR;

    using Autohand;

    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    [RequireComponent(typeof(Grabbable))]
    public class LocomotionGrabbableController : MonoBehaviour
    {
        private readonly List<XRControllerMode> controllers = new List<XRControllerMode>();

        private readonly List<RigidBodyStatus> rigidBodies = new List<RigidBodyStatus>();

        private Grabbable grabbable;

        protected void Awake()
        {
            this.grabbable = this.GetComponent<Grabbable>();
        }

        protected void OnEnable()
        {
            this.grabbable.onGrab.AddListener(this.HandleGrab);
            this.grabbable.onRelease.AddListener(this.HandleRelease);

            var locomotionProviders = LocomotionProvider.FindObjectsOfType<LocomotionProvider>(true);
            foreach (var locomotionProvider in locomotionProviders)
            {
                locomotionProvider.beginLocomotion += this.HandleBeginLocomotion;
                locomotionProvider.endLocomotion += this.HandleEndLocomotion;
            }
        }

        protected void OnDisable()
        {
            this.grabbable.onGrab.RemoveListener(this.HandleGrab);
            this.grabbable.onRelease.RemoveListener(this.HandleRelease);

            var locomotionProviders = LocomotionProvider.FindObjectsOfType<LocomotionProvider>(true);
            foreach (var locomotionProvider in locomotionProviders)
            {
                locomotionProvider.beginLocomotion -= this.HandleBeginLocomotion;
                locomotionProvider.endLocomotion -= this.HandleEndLocomotion;
            }

            foreach (var rigidBodyStatus in this.rigidBodies)
            {
                rigidBodyStatus.EndLocomotion(null);
            }

            this.rigidBodies.Clear();

            foreach (var controllerMode in this.controllers)
            {
                controllerMode.TeleportEnabled = true;
            }
        }

        private void HandleGrab(Hand hand, Grabbable g)
        {
            var controllerMode = hand.follow.GetComponentInChildren<XRControllerMode>();
            if (controllerMode != null)
            {
                controllerMode.TeleportEnabled = false;
                if (!this.controllers.Contains(controllerMode))
                {
                    this.controllers.Add(controllerMode);
                }
            }

            if (hand.body != null && !this.rigidBodies.Exists(data => object.ReferenceEquals(data.Rigidbody, hand.body)))
            {
                this.rigidBodies.Add(new RigidBodyStatus(hand.body));
            }
        }

        private void HandleRelease(Hand hand, Grabbable g)
        {
            var controllerMode = hand.follow.GetComponentInChildren<XRControllerMode>();
            if (controllerMode != null)
            {
                controllerMode.TeleportEnabled = true;
                this.controllers.Remove(controllerMode);
            }

            if (hand.body != null)
            {
                this.rigidBodies.RemoveAll(data => object.ReferenceEquals(data.Rigidbody, hand.body));
            }
        }

        private void HandleBeginLocomotion(LocomotionSystem locomotionSystem)
        {
            if (this.rigidBodies.Count > 0)
            {
                if (this.grabbable.body != null)
                {
                    this.rigidBodies.Add(new RigidBodyStatus(this.grabbable.body));
                }

                foreach (var jointedBody in this.grabbable.jointedBodies)
                {
                    this.rigidBodies.Add(new RigidBodyStatus(jointedBody));
                }

                foreach (var rigidBodyStatus in this.rigidBodies)
                {
                    rigidBodyStatus.BeginLocomotion(locomotionSystem);
                }
            }
        }

        private void HandleEndLocomotion(LocomotionSystem locomotionSystem)
        {
            if (this.rigidBodies.Count > 0)
            {
                foreach (var rigidBodyStatus in this.rigidBodies)
                {
                    rigidBodyStatus.EndLocomotion(locomotionSystem);
                }

                if (this.grabbable.body != null)
                {
                    this.rigidBodies.RemoveAll(data => object.ReferenceEquals(data.Rigidbody, this.grabbable.body));
                }

                foreach (var jointedBody in this.grabbable.jointedBodies)
                {
                    this.rigidBodies.RemoveAll(data => object.ReferenceEquals(data.Rigidbody, jointedBody));
                }
            }
        }

        private class RigidBodyStatus
        {
            private Vector3 position;

            private Quaternion rotation;

            public RigidBodyStatus(Rigidbody rigidbody)
            {
                this.Rigidbody = rigidbody;
            }

            public Rigidbody Rigidbody { get; }

            public void BeginLocomotion(LocomotionSystem locomotionSystem)
            {
                if (this.Rigidbody != null)
                {
                    this.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    this.Rigidbody.isKinematic = true;
                    this.Rigidbody.detectCollisions = false;

                    if (locomotionSystem != null)
                    {
                        this.position = locomotionSystem.xrOrigin.transform.InverseTransformPoint(this.Rigidbody.position);
                        this.rotation = Quaternion.Inverse(locomotionSystem.xrOrigin.transform.rotation) * this.Rigidbody.rotation;
                    }
                }
            }

            public void EndLocomotion(LocomotionSystem locomotionSystem)
            {
                if (this.Rigidbody != null)
                {
                    if (locomotionSystem != null)
                    {
                        this.Rigidbody.position = locomotionSystem.xrOrigin.transform.TransformPoint(this.position);
                        this.Rigidbody.rotation = locomotionSystem.xrOrigin.transform.rotation * this.rotation;
                    }

                    this.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    this.Rigidbody.isKinematic = false;
                    this.Rigidbody.detectCollisions = true;
                }
            }
        }
    }
}
