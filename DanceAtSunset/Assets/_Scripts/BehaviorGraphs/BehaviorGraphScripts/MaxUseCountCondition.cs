using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Max Use Count", story: "Used Less Than [MaxUse] Times", category: "Conditions", id: "7d58d195ff5cf6afe33658b12c95f724")]
public partial class MaxUseCountCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> MaxUse;
    private int currentUses = 0;

    public override bool IsTrue()
    {
        Debug.LogError("MAX USE COUNT WAS RUN");

        // Check if we have enough uses left
        if (currentUses < MaxUse.Value)
            return false;

        currentUses++;

        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
