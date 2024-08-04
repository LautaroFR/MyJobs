using Assets._VRN.Core.Runtime.Player;
using Assets._VRN.Core.Runtime.Utils;
using System.Collections;
using UnityEngine;

public class RadialLayout : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float height;

    [SerializeField] private Transform childContainer;

    [Tooltip("Spacing between children in degrees")]
    [SerializeField][Range(0f, 360f)] private float spacing;

    [Space(10)]
    [Tooltip("Use spawn point in scene as focused objetive")]
    [SerializeField] private bool useSpawnPoint;

    [SerializeField] private Transform focusedObjetive;

    private bool _initializationCompleted = false;

    private int _chilCount = 0;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        if (useSpawnPoint)
            focusedObjetive = FindObjectOfType<PlayerSpawnPoint>().transform;

        if (focusedObjetive != null && !useSpawnPoint)
            focusedObjetive = this.transform;

        this.transform.position = focusedObjetive.position + focusedObjetive.forward * radius;

        var dir = (this.transform.position - focusedObjetive.position);
        dir = dir.xoz().normalized;

        var newPosition = focusedObjetive.position + dir * radius;
        newPosition.y = height;
        this.transform.position = newPosition;

        var newDirection = (newPosition - focusedObjetive.position).normalized;
        this.transform.rotation = Quaternion.LookRotation(newDirection);

        _initializationCompleted = true;
    }

    private void LateUpdate()
    {
        if (_chilCount != childContainer.childCount && _initializationCompleted)
        {
            UbicateChildrens();
        }
    }

    private void UbicateChildrens()
    {
        for (int i = 0; i < childContainer.childCount; ++i)
        {
            var child = childContainer.GetChild(i) as RectTransform;

            var angle = ((-spacing / 2 * (childContainer.childCount - 1)) + i * spacing);

            var currHeadsetPosition = focusedObjetive.position;

            Vector3 dir = Quaternion.Euler(0, angle, 0) * focusedObjetive.forward;

            Vector3 pointPosition = currHeadsetPosition + dir * radius;
            pointPosition.y = this.transform.position.y;

            child.position = pointPosition;

            Vector3 targetDirection = pointPosition - focusedObjetive.position;
            targetDirection.y = 0f;

            child.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(targetDirection).eulerAngles.y, 0f);
        }
        _chilCount = childContainer.childCount;
    }
}
