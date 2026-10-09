using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Use Ability", story: "Use [Ability]", category: "Action", id: "22c497d7c57299b2261e256bfda916d9")]
public partial class UseAbilityAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyAbility> Ability;

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

        if (!abilityExecuter.TryUseAbility(Ability.Value))
            return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return abilityExecuter.IsExecutingAbility
            ? Status.Running
            : Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

