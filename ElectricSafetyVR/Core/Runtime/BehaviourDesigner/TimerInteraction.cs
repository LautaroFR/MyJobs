using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.Events;

[TaskCategory("ISVR")]

public class TimerInteraction : Action
{
    public SharedGameObject TimerGameObject;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset element;
#endif
    public SharedFloat Time;
    public Option TimerAction;

    private TimerController _timer;
    private bool _completed;
    private float _waitDuration;
    public override void OnStart()
    {
        if (TimerGameObject.Value != null)
        {
            _timer = TimerGameObject.Value.GetComponent<TimerController>();
        }

        if (_timer == null)
        {
            Debug.LogError("[TimerController] Not found");
        }

        _waitDuration = Time.Value;

        switch (TimerAction)
        {
            case Option.StartTimer:
                _timer.InitialTime(_waitDuration);
                _timer.StartTimer();
                _completed = true;
                break;
            case Option.PauseTimer:
                _timer.PauseTimer();
                _completed = true;
                break;
            case Option.ResumeTimer:
                _timer.ResumeTimer();
                _completed = true;
                break;
            case Option.OnTimeOut:
                _timer.OnTimeOut.AddListener(OnTimeFinished);
                break;
            default:
                break;  
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_completed)
            return TaskStatus.Success;

        else return TaskStatus.Running;
    }

    private void OnTimeFinished()
    {
        _completed = true;
    }
}

public enum Option
{
    StartTimer,
    PauseTimer,
    ResumeTimer,
    OnTimeOut
}