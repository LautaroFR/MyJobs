using BehaviorDesigner.Runtime;
using UnityEngine;
using TMPro;

public class DigitalUiTimerController : MonoBehaviour
{
    [SerializeField]  
    private TimerController timer;

    [SerializeField]
    private TMP_Text timerText;

    void Start()
    {
        if (timer == null)
        {
            Debug.LogError("[TimerController] Not assigned");
        }

        timer.OnTimeChange.AddListener(HandleOnTimeChanged);
    }

    private void HandleOnTimeChanged(float time)
    {
        if(!timerText.isActiveAndEnabled)
            timerText.gameObject.SetActive(true);

        int minutes = Mathf.FloorToInt(time / 60.0f);
        int seconds = Mathf.FloorToInt(time % 60.0f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
