using UnityEngine;

public class ScrewdriverController : MonoBehaviour
{
    [SerializeField]
    private float xConeAcceptance, yConeAcceptance;
    
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    public float zStopPosition;

    private DrillController _drill;

    private Transform _screw;

    void Start()
    {
        _drill = FindObjectOfType<DrillController>();
    }

    private void OnTriggerEnter(Collider other)
    {
      if(other.CompareTag("Tornillo"))
            _screw = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tornillo"))
            _screw = null;
    }

    void Update()
    {
        if(_screw != null && _drill.rotating)
        {
            float rotationX = _drill.transform.eulerAngles.x;
            float rotationY = _drill.transform.eulerAngles.y;

            float minX = 30f - (xConeAcceptance / 2);
            float maxX = 30f + (xConeAcceptance / 2);

            float minY = 180f - (yConeAcceptance / 2);
            float maxY = 180f + (yConeAcceptance / 2);

            bool isRotationXValid = rotationX >= minX && rotationX <= maxX;
            bool isRotationYValid = rotationY >= minY && rotationY <= maxY;

            if (isRotationXValid && isRotationYValid)
            {
                var displacement = moveSpeed * Time.deltaTime * _drill.triggerValue;
                var newZ = _screw.localPosition.z - displacement;

                if (newZ >= zStopPosition)
                {
                    _screw.Rotate(Vector3.forward * _drill.ScrewdriverSpeed());
                    _screw.Translate(0f, 0f, -displacement);
                }
                else
                {
                    _screw.GetComponent<CheckIfElementIsPositioned>().ForceToPositionObject = true;
                    _screw.GetComponent<CheckIfElementIsPositioned>().IsInPosition = true;
                }
            }            
        }
    }
}