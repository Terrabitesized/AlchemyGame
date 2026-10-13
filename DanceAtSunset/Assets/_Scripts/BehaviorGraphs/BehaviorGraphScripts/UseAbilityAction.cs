using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Use Ability", story: "Use [Ability] Immediately: [Immediate]", category: "Action", id: "22c497d7c57299b2261e256bfda916d9")]
public partial class UseAbilityAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyAbility> Ability;
    [SerializeReference] public BlackboardVariable<bool> Immediate;
    private EnemyAbilityExecuter abilityExecuter;

    protected override Status OnStart()
    {
        abilityExecuter = GameObject.GetComponent<EnemyAbilityExecuter>();

        if (abilityExecuter == null)
        {
            Debug.LogError(
                "UseAbilityAction requires a BaseEnemyAI component.",
                GameObject
            );

            return Status.Failure;
        }

        if (Ability == null || Ability.Value == null)
        {
            Debug.LogError(
                "UseAbilityAction has no EnemyAbility assigned.",
                GameObject
            );

            return Status.Failure;
        }

        // If this ability is already charging, wait for it.
        if (!Immediate && abilityExecuter.IsChargingAbility)
            return Status.Running;

        // Start the ability.
        if (abilityExecuter.TryUseAbility(Ability.Value, Immediate.Value))
            return Status.Running;

        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return abilityExecuter.IsChargingAbility
            ? Status.Running
            : Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

