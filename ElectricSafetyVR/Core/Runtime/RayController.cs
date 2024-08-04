using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayController : MonoBehaviour
{
    [SerializeField]
    private XRRayInteractor rightRayInteractor;
    [SerializeField]
    private XRRayInteractor leftRayInteractor;
     
    private void Start()
    {
        if (rightRayInteractor == null || leftRayInteractor == null)
        {
            Debug.LogError("[RayController] References not found");
        }
    }

    private void Update()
    {
        ActivateRays(IsAnyCanvasActive());
    }

    private bool IsAnyCanvasActive()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.isActiveAndEnabled && !canvas.CompareTag("IgnoreRays"))
            {
                return true; 
            }
        }
        return false;
    }

    private void ActivateRays(bool activateHands)
    {
        rightRayInteractor.enabled = activateHands;
        leftRayInteractor.enabled = activateHands;
    }
}
