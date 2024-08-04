using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;
using BehaviorDesigner.Runtime;
using UnityEngine;

[TaskCategory("ISVR.Teleport")]
public class CheckTeleportationCompleteAction : Action
{
    public SharedGameObject targetGameObject;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset teleportAnchor;
#endif

    public TelportationType type;

    private bool _doneTeleport;

    private GameObject _currentGameObject;

    public override void OnStart()
    {
        _doneTeleport = false;

        _currentGameObject = targetGameObject.Value;

        switch (type)
        {
            case TelportationType.Area:
                _currentGameObject.GetComponent<TeleportationArea>().teleporting.AddListener(OnTeleport);
                break;
            case TelportationType.Anchor:
                _currentGameObject.GetComponent<TeleportationAnchor>().teleporting.AddListener(OnTeleport);
                break;
            default:
                break;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_currentGameObject == null)
            return TaskStatus.Failure;

        if (_doneTeleport)
        {
            switch (type)
            {
                case TelportationType.Area:
                    _currentGameObject.GetComponent<TeleportationArea>().teleporting.RemoveListener(OnTeleport);
                    break;
                case TelportationType.Anchor:
                    _currentGameObject.GetComponent<TeleportationAnchor>().teleporting.RemoveListener(OnTeleport);
                    break;
                default:
                    break;
            }
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }

    private void OnTeleport(TeleportingEventArgs arg0)
    {
        _doneTeleport = true;
    }
}

public enum TelportationType
{
    Area,
    Anchor
}