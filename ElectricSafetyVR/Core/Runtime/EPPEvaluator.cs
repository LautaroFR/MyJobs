using System;
using System.Collections.Generic;
using System.Linq;
using Assets._VRN.Core.Runtime.Interactions;
using Assets._VRN.Core.Runtime.Utils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using VRN.GOIDs;

public class EPPEvaluator : MonoBehaviour
{
    public static EPPEvaluator Singleton { get; private set; }
    public GOIDAsset situation;

    [Header("Debug")]
    [ReadOnly]
    public bool result;

    [ReadOnly]
    public List<EPPItem> correct = new();

    [ReadOnly]
    public List<EPPItem> alternatives = new();

    [ReadOnly]
    public List<EPPItem> missing = new();

    [ReadOnly]
    public List<EPPItem> wrong = new();

    public Action onShowResult;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogError("xoxo");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }


    [Button("Evaluate", EButtonEnableMode.Playmode)]
    public bool Evaluate()
    {
        Debug.Assert(situation);
        var r = true;
        correct.Clear();
        alternatives.Clear();
        missing.Clear();
        wrong.Clear();
        foreach (var epp in EPPItem.instances)
        {
            var should = System.Array.IndexOf(epp.socket.situations, situation) != -1;
            var outline = epp.Outline;
            if (should)
            {
                if (epp.IsPositionated)
                {
                    correct.Add(epp);
                }
                else if (epp.socket.alternatives.Count != 0 && EPPItem.instances.Exists(x => 
                    x != epp && 
                    x.IsPositionated &&
                    x.socket.alternatives.Contains(epp.socket)))
                {
                    alternatives.Add(epp);
                }
                else
                {
                    r = false;
                    missing.Add(epp);
                }
            }
            else if (epp.IsPositionated)
            {
                r = false;
                wrong.Add(epp);
            }
        }
        result = r;
        return r;
    }

    public void ShowResult()
    {
        onShowResult?.Invoke();
    }
}
