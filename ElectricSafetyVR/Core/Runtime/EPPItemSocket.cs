using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Localization;
using VRN.GOIDs;

public class EPPItemSocket : MonoBehaviour
{
    [SerializeField]
    private Material mat;

    [Space]
    public LocalizedString displayName;

    [Space]
    public Transform syncParent;
    
    [SerializeField]
    private GameObject[] sync;

    [Space]
    [SerializeField]
    private GameObject[] mannequinParts;

    [Space]
    // NOTE: alternatives ignore situations, at the time it's not a problem,
    // but if we need alternatives per situation this needs to be changed
    public List<EPPItemSocket> alternatives;

    [Space]
    public GOIDAsset[] situations;
    public GOIDAsset[] tags;
    public GOIDAsset[] modelVariants;

    private void Awake()
    {
        for (int i = 0; i < alternatives.Count; i++)
        {
            var alternative = alternatives[i];
            Debug.Assert(alternative != this, "EPP | Alternatives contains it self", this);
            if (alternative && alternative.alternatives.IndexOf(this) == -1)
            {
                Debug.Assert(alternative.situations.Intersect(situations).Count() == situations.Length, "EPP | Alternatives should be set for same situations", this);
                alternative.alternatives.Add(this);
            }
        }
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            var m = r.sharedMaterials;
            for (int mi = 0; mi < m.Length; mi++)
            {
                m[mi] = mat;
            }
            r.sharedMaterials = m;
            r.enabled = false;
        }
        var otherColliders = syncParent.GetComponentsInChildren<Collider>(true);
        for (int i = otherColliders.Length - 1; i >= 0; i--)
        {
            Destroy(otherColliders[i]);
        }
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
            collider.isTrigger = true;
        }
        Destroy(GetComponentInChildren<Animator>(true));
    }

    public void ShowMannequinParts(bool show)
    {
        foreach (var go in mannequinParts)
        {
            go.SetActive(show);
        }
    }

    [Button("Sync", EButtonEnableMode.Editor)]
    public void Generate()
    {
        if (syncParent == null)
        {
            Debug.LogError("syncParent == null");
            return;
        }
        var childCount = syncParent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            var tr = syncParent.GetChild(i);
            DestroyImmediate(tr.gameObject);
        }
        foreach (var go in sync)
        {
            var ins = Instantiate(go, syncParent);
            ins.transform.SetPositionAndRotation(go.transform.position, go.transform.rotation);
            ins.name = go.name;
            ins.SetActive(true);
            go.SetActive(false);
        }
        var renderers = GetComponentsInChildren<Renderer>();
        foreach( var r in renderers)
        {
            r.enabled = true;
        }
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
            collider.isTrigger = true;
        }
    }

    public static bool Intersects(GOIDAsset[] a, GOIDAsset[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = 0; j < b.Length; j++)
            {
                if (a[i] == b[j])
                {
                    return true;
                }
            }
        }
        return false;
    }
}
