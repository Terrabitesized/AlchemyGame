using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait Game Seconds", story: "Wait for [Seconds] Game seconds", category: "Action", id: "6057acd643d649d024fc3aed6b9e316c")]
public partial class WaitGameSecondsAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Seconds;
    private float elapsedGameTime = 0f;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (elapsedGameTime < Seconds.Value)
        {
            if (CombatManager.Instance.CombatFlow)
                elapsedGameTime += Time.deltaTime;

            return Status.Running;
        }
        else
            return Status.Success;
    }

    protected override void OnEnd()
    {
        elapsedGameTime = 0f;
    }
}

