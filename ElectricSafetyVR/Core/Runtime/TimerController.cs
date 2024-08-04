using UnityEngine;
using UnityEngine.Events;

public class TimerController : MonoBehaviour
{
    private float _initialTime;
    public void InitialTime(float time) => _initialTime = time;

    public delegate void TimerEventHandler();
    public event TimerEventHandler OnTimerStart;
    public event TimerEventHandler OnTimerPause;
    public event TimerEventHandler OnTimerResume;

    public UnityEvent OnTimeOut = new UnityEvent();
    public UnityEvent OnTimerHalfway = new UnityEvent();
    public UnityEvent OnTimerTwoMinutesLeft = new UnityEvent();
    public UnityEvent OnTimerOneMinuteLeft = new UnityEvent();

    [HideInInspector]
    public UnityEvent<float> OnTimeChange = new UnityEvent<float>();

    private bool _isPaused = true;
    private bool _halfwayEventFired;
    private bool _twoMinutesLeftEventFired;
    private bool _oneMinuteLeftEventFired;

    private float _currentTime;

    private void Update()
    {
        if (!_isPaused)
        {
            _currentTime -= Time.deltaTime;

            if (_currentTime <= 0.0f)
            {
                _currentTime = 0.0f;
                OnTimeOut?.Invoke();
                TimerFinished();
            }
            else if (_currentTime <= (_initialTime / 2.0f) && !_halfwayEventFired)
            {
                OnTimerHalfway?.Invoke();
                _halfwayEventFired = true;
            }
            else if (_currentTime <= 120.0f && !_twoMinutesLeftEventFired && _initialTime > 120f)
            {
                OnTimerTwoMinutesLeft?.Invoke();
                _twoMinutesLeftEventFired = true;
            }
            else if (_currentTime <= 60.0f && !_oneMinuteLeftEventFired && _initialTime > 60f)
            {
                OnTimerOneMinuteLeft?.Invoke();
                _oneMinuteLeftEventFired = true;
            }

            UpdateTimer();
        }
    }

    public void StartTimer()
    {
        this.enabled = true;

        _currentTime = _initialTime;

        _halfwayEventFired = false;
        _twoMinutesLeftEventFired = false;
        _oneMinuteLeftEventFired = false;

        _isPaused = false;

        UpdateTimer();

        OnTimerStart?.Invoke();
    }

    public void PauseTimer()
    {
        if (this.enabled)
        {
            _isPaused = true;
            OnTimerPause?.Invoke();
        }
    }

    public void ResumeTimer()
    {
        if (this.enabled)
        {
            _isPaused = false;
            OnTimerResume?.Invoke();
        }
    }

    public void TimerFinished()
    {
        _isPaused = true;
        this.enabled = false;
    }

    private void UpdateTimer()
    {
        OnTimeChange.Invoke(_currentTime);
    }
}