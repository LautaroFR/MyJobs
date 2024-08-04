using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TargetController : MonoBehaviour
{
    public UnityEvent<TargetController> OnTimeOut = new UnityEvent<TargetController>();
    public UnityEvent<TargetController> OnReachObjetive = new UnityEvent<TargetController>();
    public UnityEvent<TargetController> OnApplying = new UnityEvent<TargetController>();

    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private int maxValue;

    private bool _completed = false;

    private float _currentValue;

    void Start()
    {
        text.text = "Target";
    }

    public void ApplyingFire(float value)
    {
        if(!_completed)
        {
            _currentValue += value;
            UpdateText(_currentValue);
            OnApplying?.Invoke(this);

            if (_currentValue >= maxValue)
            {
                ReachObjetive();
                _completed = true;
                text.text = "Completed";
            }
        }
    }

    public void UpdateText(float value)
    {
        text.text = $"{value:F0}/{maxValue}";
    }

    private void TimeOut()
    {
        OnTimeOut?.Invoke(this);
    }

    private void ReachObjetive()
    {
        OnReachObjetive?.Invoke(this);
    }
}
