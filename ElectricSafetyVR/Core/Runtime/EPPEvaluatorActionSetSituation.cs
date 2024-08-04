using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using VRN.GOIDs;

[TaskCategory("ISVR/(ISVR) EPP Evaluator")]
[TaskName("Set Situation")]
public class EPPEvaluatorActionSetSituation : Action
{
    public override string FriendlyName { get => string.IsNullOrEmpty(base.FriendlyName) ? "Set Situation" : base.FriendlyName; set => base.FriendlyName = value; }

    public SharedObject id;

    public override void OnStart()
    {
        var eval = EPPEvaluator.Singleton;
        Debug.Assert(id.Value is GOIDAsset);
        eval.situation = id.Value as GOIDAsset;
    }
}
