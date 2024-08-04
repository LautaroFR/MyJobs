namespace Assets._ISVR.Core.Runtime.Video
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Video;

    [RequireComponent(typeof(VideoPlayer))]
    public class VideoRenderTexture : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        private int width = 1920;

        [SerializeField]
        [Min(1)]
        private int height = 1080;

        [Space]

        [SerializeField]
        private RawImage targetRawImage;

        private VideoPlayer videoPlayer;

        private RenderTexture renderTexture;

        protected void Awake()
        {
            this.videoPlayer = this.GetComponent<VideoPlayer>();
        }

        protected void Start()
        {
            this.renderTexture = new RenderTexture(this.width, this.height, 0);
            if (this.renderTexture.Create())
            {
                this.videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                this.videoPlayer.targetTexture = this.renderTexture;

                if (this.targetRawImage != null)
                {
                    this.targetRawImage.texture = this.renderTexture;
                }
            }
        }

        protected void OnDestroy()
        {
            if (this.renderTexture != null)
            {
                this.renderTexture.Release();
                RenderTexture.Destroy(this.renderTexture);
            }
        }
    }
}
