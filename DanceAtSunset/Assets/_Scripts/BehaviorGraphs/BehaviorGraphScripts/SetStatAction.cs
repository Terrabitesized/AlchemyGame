using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Stat", story: "Set [StatType] to [Value]", category: "Action", id: "820878354102837fa2aad374f003c962")]
public partial class SetStatAction : Action
{
    [SerializeReference] public BlackboardVariable<string> StatType;
    [SerializeReference] public BlackboardVariable<int> Value;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

