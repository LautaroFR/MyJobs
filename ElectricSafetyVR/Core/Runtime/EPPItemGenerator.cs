using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EPPItemGenerator : MonoBehaviour
{
    public Transform socketsParent;
    public EPPItem eppPrefab;

    [Button("Sync", EButtonEnableMode.Editor)]
    public void Sync()
    {
#if UNITY_EDITOR
        var epps = GetComponentsInChildren<EPPItem>();
        var sockets = socketsParent.GetComponentsInChildren<EPPItemSocket>();
        for (var i = 0; i < sockets.Length; i++)
        {
            var socket = sockets[i];
            var eppIndex = Array.FindIndex(epps, x => x.socket == socket);
            if (eppIndex != -1)
            {
                epps[eppIndex].transform.SetSiblingIndex(socket.transform.GetSiblingIndex());
                epps[eppIndex].name = socket.name.Replace("-SOCKET", "-INTERACTABLE");
                epps[eppIndex].Sync();
                continue;
            }
            var ins = PrefabUtility.InstantiatePrefab(eppPrefab, gameObject.scene) as EPPItem;
            ins.transform.parent = transform;
            ins.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ins.socket = socket;
            ins.Sync();
        }
#endif
    }
}
