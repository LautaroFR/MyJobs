using Autohand;
using UnityEngine;
using UnityEngine.Events;

public class CheckIfElementIsPositioned : MonoBehaviour
{
    public delegate void VariableChangedEventHandler();
    public event VariableChangedEventHandler VariableChanged;
    public UnityEvent OnPosition;

    [HideInInspector]
    public bool ForceToPositionObject = false;

    public Collider trayCollider;

    private bool _isInPosition = false;

    private bool _isColliding = false;

    private bool _isGrabbing;

    public bool IsInPosition
    {
        get { return _isInPosition; }
        set
        {
            if (_isInPosition != value)
            {
                _isInPosition = value;
                OnVariableChanged();
                OnPosition.Invoke();
            }
        }
    }

    private void Start()
    {
        if (GetComponent<Grabbable>() != null)
        {
            GetComponent<Grabbable>().onGrab.AddListener(GrabbingEvent);
            GetComponent<Grabbable>().onRelease.AddListener(ReleasingEvent);
        }
    }

    private void ReleasingEvent(Hand arg0, Grabbable arg1) => _isGrabbing = false;

    private void GrabbingEvent(Hand arg0, Grabbable arg1) => _isGrabbing = true;

    private void Update()
    {
        if (_isColliding == true && !_isGrabbing)
            IsInPosition = true;
        else
        {
            if (!ForceToPositionObject)
                IsInPosition = false;
        }
    }

    private void OnVariableChanged() => VariableChanged?.Invoke();

    private void OnTriggerEnter(Collider other)
    {
        if (other == trayCollider)
            _isColliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == trayCollider)
            _isColliding = false;
    }
}
