using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Enemy Count", story: "Enemy Count Less Than [NumOfEnemies]", category: "Conditions", id: "0d4ce1a7db6cda51c2ea6161cb632197")]
public partial class EnemyCountCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> NumOfEnemies;

    // Combat refs
    private CombatManager combatManager;

    public override bool IsTrue()
    {
        if (combatManager == null)
            return false;

        return combatManager.GetEnemyCount() < NumOfEnemies;
    }

    public override void OnStart()
    {
        combatManager = CombatManager.Instance;
    }

    public override void OnEnd()
    {
    }
}
