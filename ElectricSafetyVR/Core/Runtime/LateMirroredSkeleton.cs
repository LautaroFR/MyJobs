using UnityEngine;
using UnityEngine.Assertions;

    [DefaultExecutionOrder(400)]
    public class LateMirroredSkeleton : MonoBehaviour
    {
        [System.Serializable]
        public class MirroredBonePair
        {
            [HideInInspector] 
            public string Name;

            public Transform OriginalBone;

            public Transform MirroredBone;
        }

        [SerializeField]
        protected MirroredBonePair[] _mirroredBonePairs;

        private void Awake()
        {
            foreach (var mirroredBonePair in _mirroredBonePairs)
            {
                Assert.IsNotNull(mirroredBonePair.OriginalBone);
                Assert.IsNotNull(mirroredBonePair.MirroredBone);
            }
        }

        private void LateUpdate()
        {

                foreach (var bonePair in _mirroredBonePairs)
                {
                bonePair.MirroredBone.localPosition = Vector3.zero;
                bonePair.MirroredBone.localRotation = Quaternion.identity;
                    bonePair.MirroredBone.position = bonePair.OriginalBone.position;
                    bonePair.MirroredBone.rotation = bonePair.OriginalBone.rotation;
                }            
        }
    }