namespace Assets._ISVR.Core.Runtime
{
    using System;
    using System.Runtime.InteropServices;

    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.LowLevel;
    using UnityEngine.Rendering;

    public class CylinderRenderer : MonoBehaviour
    {
        [SerializeField]
        [Min(5)]
        private int sides = 10;

        [SerializeField]
        [Min(0.01f)]
        private float radius = 1;

        [Space]

        [SerializeField]
        private bool useWorldSpace = true;

        [SerializeField]
        private Vector3[] positions = new Vector3[0];

        private Vector3[] vertices;

        private Vector3[] forwards;

        private Vector3[] upwards;

        private Vector3[] sidewards;

        private Transform thisTransform;

        private Mesh mesh;

        private MeshFilter meshFilter;

        private MeshRenderer meshRenderer;

        private bool dirty;

        private Vector3[] circlePositions;

        private Vector3 forward;

        private Vector3 sideward;

        private Vector3 upward;

        private Vector3 start;

        private Vector3 end;

        private float angleStep;

        private Vector3 center;

        private Vector3 circlePosition;

        public int Sides
        {
            get => this.sides;
            set
            {
                value = Mathf.Max(5, value);
                if (value != this.sides)
                {
                    this.sides = value;
                    this.dirty = true;
                }
            }
        }

        public float Radius
        {
            get => this.radius;
            set
            {
                value = Mathf.Max(0.01f, value);
                if (Math.Abs(value - this.radius) > Mathf.Epsilon)
                {
                    this.radius = value;
                    this.dirty = true;
                }
            }
        }

        public bool UseWorldSpace
        {
            get => this.useWorldSpace;
            set
            {
                if (this.useWorldSpace != value)
                {
                    this.useWorldSpace = value;
                    this.dirty = true;
                }
            }
        }

        public int PositionCount
        {
            get => this.positions.Length;
            set
            {
                value = Mathf.Max(0, value);
                if (this.positions.Length != value)
                {
                    Array.Resize(ref this.positions, value);
                    Array.Resize(ref this.forwards, value);
                    Array.Resize(ref this.upwards, value);
                    Array.Resize(ref this.sidewards, value);

                    this.dirty = true;
                }
            }
        }

        public Vector3 GetPosition(int index)
        {
            if (index >= 0 && index < this.positions.Length)
            {
                return this.positions[index];
            }

            return default;
        }

        public void SetPosition(int index, Vector3 position)
        {
            if (index >= 0 && index < this.positions.Length)
            {
                this.positions[index] = position;
                this.dirty = true;
            }
        }

        public void GetPositions([Out] Vector3[] arrayPositions)
        {
            if (arrayPositions == null)
            {
                throw new ArgumentNullException(nameof(arrayPositions));
            }

            arrayPositions = this.positions;
        }

        public void SetPositions([NotNull] Vector3[] arrayPositions)
        {
            this.positions = arrayPositions ?? throw new ArgumentNullException(nameof(arrayPositions));

            Array.Resize(ref this.forwards, this.positions.Length);
            Array.Resize(ref this.upwards, this.positions.Length);
            Array.Resize(ref this.sidewards, this.positions.Length);

            this.dirty = true;
        }

        protected void Awake()
        {
            this.thisTransform = this.transform;

            this.meshFilter = this.GetComponent<MeshFilter>();
            if (this.meshFilter == null)
            {
                this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
            }

            this.meshRenderer = this.GetComponent<MeshRenderer>();
            if (this.meshRenderer == null)
            {
                this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
            }

            this.mesh = new Mesh { indexFormat = IndexFormat.UInt32 };
            this.mesh.MarkDynamic();

            this.meshFilter.mesh = this.mesh;

            this.forwards = new Vector3[this.positions.Length];
            this.upwards = new Vector3[this.positions.Length];
            this.sidewards = new Vector3[this.positions.Length];
        }

        protected void OnEnable()
        {
            this.meshRenderer.enabled = true;
        }

        protected void OnDisable()
        {
            this.meshRenderer.enabled = false;
        }

        protected void OnValidate()
        {
            this.dirty = Application.isPlaying;
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                if (!this.useWorldSpace)
                {
                    UnityEditor.Handles.matrix = this.transform.localToWorldMatrix;
                }

                for (var i = 0; i < this.positions.Length; i++)
                {
                    var position = this.positions[i];
                    var rotation = Quaternion.LookRotation(this.forwards[i], this.upwards[i]);

                    UnityEditor.Handles.SphereHandleCap(0, position, Quaternion.identity, 0.005f, EventType.Repaint);
                    UnityEditor.Handles.PositionHandle(position, rotation);
                }
            }
        }
#endif
        protected void Update()
        {
            if (!this.useWorldSpace && this.thisTransform.hasChanged)
            {
                this.dirty = true;
                this.thisTransform.hasChanged = false;
            }

            if (this.dirty)
            {
                this.dirty = false;

                this.GenerateMesh();
            }
        }

        private void GenerateMesh()
        {
            var verticesLength = this.sides * this.positions.Length;
            if (this.vertices == null || this.vertices.Length != verticesLength)
            {
                this.vertices = new Vector3[verticesLength];
                
                this.mesh.Clear();
                this.mesh.vertices = this.vertices;
                this.mesh.uv = this.GenerateUVs();
                this.mesh.triangles = this.GenerateTriangles();
            }

            if (this.circlePositions == null || this.circlePositions.Length != this.sides)
            {
                this.circlePositions = new Vector3[this.sides];
            }

            this.angleStep = (2 * Mathf.PI) / this.sides;

            var currentVertexIndex = 0;
            for (int i = 0; i < this.positions.Length; i++)
            {
                this.CalculateCirclePositions(i, this.circlePositions);
                foreach (var vertex in this.circlePositions)
                {
                    this.vertices[currentVertexIndex++] = this.useWorldSpace ? this.thisTransform.InverseTransformPoint(vertex) : vertex;
                }
            }

            this.mesh.vertices = this.vertices;
            this.mesh.RecalculateNormals();
            this.mesh.RecalculateBounds();
            this.mesh.RecalculateTangents();
            this.mesh.RecalculateUVDistributionMetrics();
        }

        private Vector2[] GenerateUVs()
        {
            var uvs = new Vector2[this.positions.Length * this.sides];

            for (int segment = 0; segment < this.positions.Length; segment++)
            {
                for (int side = 0; side < this.sides; side++)
                {
                    var vertexIndex = (segment * this.sides) + side;
                    var u = side / (this.sides - 1f);
                    var v = segment / (this.positions.Length - 1f);

                    uvs[vertexIndex] = new Vector2(u, v);
                }
            }

            return uvs;
        }

        private int[] GenerateTriangles()
        {
            // Two triangles and 3 vertices
            var triangles = new int[this.positions.Length * this.sides * 2 * 3];

            var currentIndex = 0;
            for (int segment = 1; segment < this.positions.Length; segment++)
            {
                for (int side = 0; side < this.sides; side++)
                {
                    var vertexIndex = (segment * this.sides) + side;
                    var previousVertexIndex = vertexIndex - this.sides;

                    // Triangle one
                    triangles[currentIndex++] = previousVertexIndex;
                    triangles[currentIndex++] = (side == this.sides - 1) ? (vertexIndex - (this.sides - 1)) : (vertexIndex + 1);
                    triangles[currentIndex++] = vertexIndex;

                    // Triangle two
                    triangles[currentIndex++] = (side == this.sides - 1) ? (previousVertexIndex - (this.sides - 1)) : (previousVertexIndex + 1);
                    triangles[currentIndex++] = (side == this.sides - 1) ? (vertexIndex - (this.sides - 1)) : (vertexIndex + 1);
                    triangles[currentIndex++] = previousVertexIndex;
                }
            }

            return triangles;
        }

        private void CalculateCirclePositions(int index, [Out] Vector3[] outPositions)
        {
            if (index == 0)
            {
                if (this.positions.Length > 1)
                {
                    this.start = this.positions[1];
                    this.end = this.positions[0];
                }
                else
                {
                    this.start = Vector3.zero;
                    this.end = Vector3.forward;
                }
            }
            else
            {
                this.start = this.positions[index];
                this.end = this.positions[index - 1];
            }

            this.forward.x = this.start.x - this.end.x;
            this.forward.y = this.start.y - this.end.y;
            this.forward.z = this.start.z - this.end.z;
            this.forward.Normalize();

            this.sideward = Vector3.Cross(this.forward, index > 0 ? this.upwards[index - 1] : this.transform.up);
            this.sideward.Normalize();

            this.upward = Vector3.Cross(this.forward, this.sideward);
            this.upward.Normalize();

            if (index > 0)
            {
                if (Vector3.Dot(this.upward, this.upwards[index - 1]) < 0)
                {
                    this.upward = -this.upward;
                }

                if (Vector3.Dot(this.sideward, this.sidewards[index - 1]) < 0)
                {
                    this.sideward = -this.sideward;
                }
            }

            this.forwards[index] = this.forward;
            this.upwards[index] = this.upward;
            this.sidewards[index] = this.sideward;

            var angle = 0f;
            this.center = this.positions[index];

            for (int i = 0; i < this.sides; i++)
            {
                var x = Mathf.Cos(angle) * this.radius;
                var y = Mathf.Sin(angle) * this.radius;

                this.circlePosition.x = this.center.x + (this.sideward.x * x) + (this.upward.x * y);
                this.circlePosition.y = this.center.y + (this.sideward.y * x) + (this.upward.y * y);
                this.circlePosition.z = this.center.z + (this.sideward.z * x) + (this.upward.z * y);

                outPositions[i] = this.circlePosition;

                angle += this.angleStep;
            }
        }
    }
}