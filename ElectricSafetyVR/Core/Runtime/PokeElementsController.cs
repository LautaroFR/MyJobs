using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PokeElementsController : MonoBehaviour
{
    [Header("Windmill Setup")]
    [SerializeField]
    private float speedIncrease;

    [SerializeField]
    private float speedDecrease;

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private Transform blades;

    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AnimationCurve deaccelerationCurve;

    [SerializeField]
    private XRSimpleInteractable continuousButton;

    [Header("House Setup")]
    [SerializeField]
    private Transform houseLightsOffPanels;

    [SerializeField]
    private XRSimpleInteractable switchButton;

    private bool _continuousPressed;

    private float _windmillInput = 0f;

    private bool _turnedOn;

    void Start()
    {
        continuousButton.hoverEntered.AddListener(ContinuousButtonPress);
        continuousButton.hoverExited.AddListener(ContinuousButtonRelease);
        switchButton.hoverEntered.AddListener(SwitchButtonPress);
    }

    private void SwitchButtonPress(HoverEnterEventArgs arg0)
    {
        _turnedOn = !_turnedOn;
        houseLightsOffPanels.gameObject.SetActive(!_turnedOn);
    }

    private void ContinuousButtonRelease(HoverExitEventArgs arg0) => _continuousPressed = false;

    private void ContinuousButtonPress(HoverEnterEventArgs arg0) => _continuousPressed = true;

    void Update()
    {
        blades.Rotate(Vector3.forward, RotateVelocity());
    }

    public float RotateVelocity()
    {     
        if(_continuousPressed) 
        { 
            _windmillInput = Mathf.Clamp01(_windmillInput + speedIncrease * Time.deltaTime);
            return accelerationCurve.Evaluate(_windmillInput) * maxSpeed;
        }        
        
        _windmillInput = Mathf.Clamp01(_windmillInput - speedDecrease * Time.deltaTime);
        return deaccelerationCurve.Evaluate(_windmillInput) * maxSpeed;                
    }
}
