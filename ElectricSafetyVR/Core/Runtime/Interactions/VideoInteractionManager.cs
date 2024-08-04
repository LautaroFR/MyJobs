using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

public class VideoInteractionManager : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;

    [SerializeField]
    private Button continueButton;

    [SerializeField]
    private Button restartButton;
    
    [SerializeField]
    private UnityEvent onPlayed = new UnityEvent();

    [SerializeField]
    private UnityEvent onFinished = new UnityEvent();

    [SerializeField]
    private UnityEvent onStopped = new UnityEvent();

    public enum StateEnum
    {
        Stopped,

        Playing,

        Finished,

        Continue
    }

    public StateEnum State { get; private set; } = StateEnum.Stopped;

    public UnityEvent OnPlayed => this.onPlayed;

    public UnityEvent OnFinished => this.onFinished;

    public UnityEvent OnStopped => this.onStopped;

    private void Start()
    {
        restartButton.onClick.AddListener(OnRestartButton);
        continueButton.onClick.AddListener(OnContinueButton);
    }

    public void Play()
    {
        if (this.State != StateEnum.Playing)
        {
            this.PlayInteraction();

            this.State = StateEnum.Playing;
            this.onPlayed.Invoke();
        }
    }

    public void Stop(bool force = false)
    {
        if (this.State == StateEnum.Playing || force)
        {
            this.StopInteraction();

            this.State = StateEnum.Stopped;
            this.onStopped.Invoke();
        }
    }

    public void ForceFinish()
    {
        this.OnInteractionFinished();
    }

    protected void OnInteractionFinished()
    {
        if (this.State == StateEnum.Playing)
        {
            this.State = StateEnum.Finished;
            this.onFinished.Invoke();

            continueButton.interactable = true;
            restartButton.interactable = true;
        }
    }

    private void PlayInteraction()
    {
        this.playableDirector.initialTime = 0;

        this.playableDirector.stopped += this.HandlePlayableStopped;
        this.playableDirector.Play();

        continueButton.interactable = false;
        restartButton.interactable = false;
    }

    private void StopInteraction()
    {
        this.playableDirector.stopped -= this.HandlePlayableStopped;

        this.playableDirector.initialTime = 0;
        this.playableDirector.Stop();
        this.playableDirector.DeferredEvaluate();
    }

    private void HandlePlayableStopped(PlayableDirector obj)
    {
        this.playableDirector.stopped -= this.HandlePlayableStopped;

        this.OnInteractionFinished();
    }

    private void OnRestartButton()
    {
        this.Play();
    }

    private void OnContinueButton()
    {
        if (this.State == StateEnum.Finished)
        {
            this.State = StateEnum.Continue;
        }
    }
}

