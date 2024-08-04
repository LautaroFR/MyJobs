using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRN.GOIDs;

public class EPPModelVariant : MonoBehaviour
{
    public bool startVisible;

    public GOIDAsset id;

    public GOIDAsset[] variants;

    private static List<EPPModelVariant> instances = new();

    private void Awake()
    {
        instances.Add(this);
        gameObject.SetActive(startVisible);
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }

    public static void Show(GOIDAsset id, bool show)
    {
        foreach (var instance in instances)
        {
            if (instance.id == id)
            {
                instance.gameObject.SetActive(show);
            }
            else if (instance.variants.Contains(id))
            {
                instance.gameObject.SetActive(!show);
            }
        }
    }

    public static void Show(GOIDAsset[] ids, bool show)
    {
        foreach (var id in ids)
        {
            Show(id, show);
        }
    }
}
