using Michsky.MUIP;
using System.Collections.Generic;
using UnityEngine;

public class PositionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToEnable;

    [SerializeField]
    private List<CheckIfElementIsPositioned> objectsList = new List<CheckIfElementIsPositioned>();

    void Start()
    {
        objectToEnable.SetActive(false);
    }

    void Update()
    {
        if (objectsList == null || objectsList.Count == 0)
            return;

        objectToEnable.SetActive(AreAllObjectsInPosition());
    }

    private bool AreAllObjectsInPosition()
    {
        bool allObjectsInPosition = true;

        foreach (CheckIfElementIsPositioned objet in objectsList)
        {
            if (!objet.IsInPosition)
            {
                allObjectsInPosition = false;
                break;
            }
        }
        return allObjectsInPosition;
    }
}

