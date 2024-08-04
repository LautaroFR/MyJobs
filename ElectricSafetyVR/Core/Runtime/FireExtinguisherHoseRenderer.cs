namespace Assets._ISVR.Core.Runtime
{
    using Assets.BezierCurve.Scripts;
    using Assets.BezierCurve.Scripts.Utils;

    using UnityEngine;

    [RequireComponent(typeof(CylinderRenderer))]
    public class FireExtinguisherHoseRenderer : MonoBehaviour
    {
        [SerializeField]
        private BezierPoint startBezierPoint;

        [SerializeField]
        private BezierPoint middleBezierPoint;

        [SerializeField]
        private BezierPoint endBezierPoint;

        [SerializeField]
        [Min(2)]
        private int resolution = 10;

        [SerializeField]
        [Min(0.01f)]
        private float distanceBetweenSmoothedPoints = 0.05f;

        private CylinderRenderer cylinderRenderer;

        protected void Awake()
        {
            this.cylinderRenderer = this.GetComponent<CylinderRenderer>();
        }

        protected void Update()
        {
            this.middleBezierPoint.transform.position =
                (this.startBezierPoint.GlobalFirstControlPoint + this.endBezierPoint.GlobalSecondControlPoint) * 0.5f;

            this.middleBezierPoint.GlobalSecondControlPoint = this.startBezierPoint.GlobalFirstControlPoint;
            this.middleBezierPoint.GlobalFirstControlPoint = this.endBezierPoint.GlobalSecondControlPoint;

            var length = BezierUtils.CalculateApproximateLength(this.startBezierPoint, this.middleBezierPoint, this.resolution);
            var stepCount = (int)(length / this.distanceBetweenSmoothedPoints);

            var positionCount = stepCount + 1;
            if (this.cylinderRenderer.PositionCount < positionCount)
            {
                this.cylinderRenderer.PositionCount = positionCount;
            }

            for (int i = 0; i <= stepCount; i++)
            {
                this.cylinderRenderer.SetPosition(
                    i,
                    BezierUtils.GetPoint(this.startBezierPoint, this.middleBezierPoint, (float)i / stepCount));
            }

            length = BezierUtils.CalculateApproximateLength(this.middleBezierPoint, this.endBezierPoint, this.resolution);
            stepCount = (int)(length / this.distanceBetweenSmoothedPoints);

            this.cylinderRenderer.PositionCount = positionCount + stepCount;

            for (int i = 1; i <= stepCount; i++)
            {
                this.cylinderRenderer.SetPosition(
                    positionCount + i - 1,
                    BezierUtils.GetPoint(this.middleBezierPoint, this.endBezierPoint, (float)i / stepCount));
            }
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            if (this.middleBezierPoint != null)
            {
                if (this.startBezierPoint != null)
                {
                    BezierUtils.DrawCurve(this.startBezierPoint, this.middleBezierPoint, this.resolution);
                }

                if (this.endBezierPoint != null)
                {
                    BezierUtils.DrawCurve(this.middleBezierPoint, this.endBezierPoint, this.resolution);
                }
            }
        }
#endif

        private int SetCylinderPositions(BezierPoint start, BezierPoint end, int startIndex)
        {
            var length = BezierUtils.CalculateApproximateLength(start, end, this.resolution);
            var stepCount = (int)(length / this.distanceBetweenSmoothedPoints);

            var positionCount = startIndex + stepCount + 1;
            if (this.cylinderRenderer.PositionCount < positionCount)
            {
                this.cylinderRenderer.PositionCount = positionCount;
            }

            for (int j = startIndex == 0 ? 0 : 1; j <= stepCount; j++)
            {
                this.cylinderRenderer.SetPosition(
                    startIndex + j,
                    BezierUtils.GetPoint(start, end, (float)j / stepCount));
            }

            return positionCount;
        }
    }
}
