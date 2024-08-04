using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("ISVR/(ISVR) EPP Evaluator")]
[TaskName("Evaluate")]
public class EPPEvaluatorActionEvaluate : Action
{
    public override string FriendlyName { get => string.IsNullOrEmpty(base.FriendlyName) ? "Evaluate" : base.FriendlyName; set => base.FriendlyName = value; }

    public SharedBool storeValue;

    public override void OnStart()
    {
        var eval = EPPEvaluator.Singleton;
        storeValue = eval.Evaluate();
        eval.ShowResult();
    }
}
