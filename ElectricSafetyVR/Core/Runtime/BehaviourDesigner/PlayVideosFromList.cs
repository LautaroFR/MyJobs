namespace Assets._VRN.Core.Runtime.BehaviorDesigner.Interaction
{
    using global::BehaviorDesigner.Runtime;
    using global::BehaviorDesigner.Runtime.Tasks;

    [TaskCategory("VRN/Interaction")]
    public class PlayVideosFromList : Action
    {
        public SharedObject chapterData;
        public SharedGameObject videoPlayerGameObject;
        public float waitTime;

#if VRN_GOIDS_INCLUDE_DEPRECATED
        [Header("DEPRECATED")]
        public VRN.GOIDs.BehaviorDesigner.SharedGOIDAsset GoidAsset;
#endif

        private VideoPlayerManager _videoPlayer;
        private bool _completed;

        public override void OnStart()
        {
            var data = chapterData.Value as ChapterData;

            if (videoPlayerGameObject.Value != null)
            {
                _videoPlayer = videoPlayerGameObject.Value.GetComponent<VideoPlayerManager>();
            }
            
            _videoPlayer.waitTime = waitTime;
            _videoPlayer.OnVideoEnds.AddListener(OnVideosEnds);
            _completed = false;
            StartCoroutine(_videoPlayer.VideoInteraction(data.orderedVideosTimelines));
        }

        private void OnVideosEnds()
        {
            _completed = true;
        }

        public override TaskStatus OnUpdate()
        {
            if (this.chapterData == null || _videoPlayer == null)
            {
                return TaskStatus.Failure;
            }

            if (_completed)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }
    }
}