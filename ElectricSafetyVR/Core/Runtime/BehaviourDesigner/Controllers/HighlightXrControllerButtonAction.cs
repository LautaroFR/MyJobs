using VRN.GOIDs;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("ISVR.Controllers")]
public class HighlightXrControllerButtonAction : Action
{
    public List<GOIDAsset> buttonsToHighlight = new List<GOIDAsset>();

    [SerializeField]
    private bool highlight;

    [SerializeField]
    private Material highlightMaterial;

    [SerializeField]
    private Material originalMaterial;

    private SkinnedMeshRenderer _rend;

    public override void OnStart()
    {
        foreach (GOIDAsset button in buttonsToHighlight)
        {
            GOID.TryFind(button, out var obj);
            _rend = obj.transform.GetComponent<SkinnedMeshRenderer>();
            if (highlight)
            {
                _rend.material = highlightMaterial;
            }
            else
                _rend.material = originalMaterial;
        }       
    }

    public override TaskStatus OnUpdate()
    {
        if (buttonsToHighlight == null || highlightMaterial == null)
        {
            Debug.LogError("[HighlightButton] No references found");
            return TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Success;
        }    
    }
}
