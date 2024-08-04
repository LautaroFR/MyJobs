using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector timelineNormal;
    public PlayableDirector timelineFallo;

    private void Start()
    {
        timelineNormal.stopped += OnTimeline1Finished;

        if (timelineFallo == null || timelineNormal == null)
            Debug.LogError("[Timeline Controller] Timelines not assigned");

        timelineFallo.gameObject.SetActive(false);
    }

    private void OnTimeline1Finished(PlayableDirector director)
    {
        if (director == timelineNormal)
        {
            timelineFallo.gameObject.SetActive(true);
            timelineFallo.Play();
        }
    }

    public void EmergencyStop()
    {
        timelineFallo.Stop();
    }
}