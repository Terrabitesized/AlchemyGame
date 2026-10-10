using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Limited Use Ability", story: "Use [Ability] a Max Of [Count] Times", category: "Action", id: "78b7b95f0a94b7569f6a222454a978de")]
public partial class LimitedUseAbilityAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyAbility> Ability;
    [SerializeReference] public BlackboardVariable<int> Count;

    private int currentUses = 0;
    private EnemyAbilityExecuter abilityExecuter;

    protected override Status OnStart()
    {
        // Check if we have enough uses left
        if(currentUses >= Count.Value)
            return Status.Failure;

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
        currentUses++;
    }
}

