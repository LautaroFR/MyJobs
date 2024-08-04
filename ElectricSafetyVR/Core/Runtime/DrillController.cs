using Autohand;
using UnityEngine;

[RequireComponent(typeof(Grabbable))]
public class DrillController : MonoBehaviour
{
    public float ScrewdriverMaxSpeed;

    [HideInInspector]
    public bool rotating;

    [HideInInspector]
    public float triggerValue;

    [SerializeField]
    private Transform screwdriver;

    [SerializeField]
    private AudioSource audioSource;

    private bool _grabbing;

    private Grabbable _grabbable;

    private bool squeezingTrigger;
    private bool _audioPlaying = false;

    private void Start()
    {
        _grabbable = GetComponent<Grabbable>();

        _grabbable.onGrab.AddListener(GrabbingEvent);
        _grabbable.onRelease.AddListener(ReleasingEvent);
    }

    private void ReleasingEvent(Hand arg0, Grabbable arg1)
    {
        _grabbing = false;
    }

    private void GrabbingEvent(Hand arg0, Grabbable arg1)
    {
        _grabbing = true;
    }

    public void PressTrigger()
    {
        squeezingTrigger = true;
        rotating = true;
    }

    public void ReleaseTrigger()
    {
        squeezingTrigger = false;
        rotating = false;
    }

    private void Update()
    {
        if (triggerValue > 0 && _grabbing)
        {
            audioSource.pitch = triggerValue;
            screwdriver.transform.Rotate(Vector3.forward * ScrewdriverSpeed());
            if (!_audioPlaying)
            {
                audioSource.Play();
                _audioPlaying = true;
            }
        }

        if (triggerValue <= 0 || !_grabbing)
        {
            _audioPlaying = false;
            audioSource.Stop();
        }
    }

    public float ScrewdriverSpeed() => Time.deltaTime * ScrewdriverMaxSpeed * triggerValue;
}