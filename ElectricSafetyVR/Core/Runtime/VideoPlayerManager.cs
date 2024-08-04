using Assets._VRN.Core.Runtime.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class VideoPlayerManager : MonoBehaviour
{
    public UnityEvent OnVideoEnds = new UnityEvent();

    public float waitTime = 0.5f;

    private BaseInteraction _videoInteraction;

    // NOTE: Remember to assign Bindings in Scene on videoHolder GO for each Timeline video.
    public IEnumerator VideoInteraction(List<PlayableAsset> videosTimelines)
    {
        foreach (var video in videosTimelines)
        {
            this.gameObject.SetActive(true);

            this.GetComponent<PlayableDirector>().playableAsset = video;
            _videoInteraction = this.GetComponent<BaseInteraction>();

            yield return null;

            _videoInteraction.Play();

            yield return null;

            if (this._videoInteraction != null)
            {
                while (_videoInteraction.State != BaseInteraction.StateEnum.Finished)
                {
                    yield return null;
                }
            }       
        }

        this.gameObject.SetActive(false);

        OnVideoEnds?.Invoke();
    }
}