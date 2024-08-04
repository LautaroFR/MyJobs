using VRN.GOIDs;
using UnityEngine;

public class HandsController : MonoBehaviour
{
    [SerializeField]
    private GOIDAsset rightHand;
    [SerializeField]
    private GOIDAsset leftHand;
    [SerializeField]
    [Tooltip("The No-Hand item to right hand slot: Controller, glove")]
    private GOIDAsset rightController;
    [SerializeField]
    [Tooltip("The No-Hand item to left hand slot: Controller, glove")]
    private GOIDAsset leftController;

    private void Start()
    {
        if(rightHand == null || leftHand == null || rightController == null || leftController == null)
        {
            Debug.LogError("[HandsController] References not found");
        }
    }

    public void ActivateHands(bool activateHands)
    {
        GOID.TryFind(rightHand, out var rHand);
        GOID.TryFind(leftHand, out var lHand);
        GOID.TryFind(rightController, out var rController);
        GOID.TryFind(leftController, out var lController);

        rHand.SetActive(activateHands);
        lHand.SetActive(activateHands);
        rController.SetActive(!activateHands);
        lController.SetActive(!activateHands);
    }
}
