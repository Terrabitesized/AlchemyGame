using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Health Threshold", story: "Health below [Percent]%", category: "Conditions", id: "1f3583de63dc953c96f7faa7343dfbe4")]
public partial class HealthThresholdCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> Percent;

    // Enemy Stats ref
    private Stats EnemyStats;

    public override bool IsTrue()
    {
        if (EnemyStats == null)
            return false;

        return ((float) EnemyStats.CurrentHealth / EnemyStats.MaxHealth) <= Percent.Value / 100f;
    }

    public override void OnStart()
    {
        if (GameObject.GetComponent<EnemyStats>() != null)
            EnemyStats = GameObject.GetComponent<EnemyStats>().Stats;
    }

    public override void OnEnd()
    {
    }
}
