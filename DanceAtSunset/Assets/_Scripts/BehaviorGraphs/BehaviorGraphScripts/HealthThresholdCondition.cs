using System;
using Unity.Behavior;
using UnityEngine;

public enum EnemyHealthComparison
{
    LessThan,
    GreaterThan,
    Equal
}

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Health Threshold", story: "Health [Comparison] [Percent]%", category: "Conditions", id: "1f3583de63dc953c96f7faa7343dfbe4")]
public partial class HealthThresholdCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> Percent;
    [SerializeReference] public BlackboardVariable<EnemyHealthComparison> Comparison;
    // Enemy Stats ref
    private Stats EnemyStats;

    public override bool IsTrue()
    {
        if (EnemyStats == null)
            return false;

        float enemyHealthPercent = (float)EnemyStats.CurrentHealth / EnemyStats.MaxHealth * 100f;

        switch (Comparison.Value)
        {
            case EnemyHealthComparison.LessThan:
                return enemyHealthPercent < Percent.Value;

            case EnemyHealthComparison.GreaterThan:
                return enemyHealthPercent > Percent.Value;

            case EnemyHealthComparison.Equal:
                return Mathf.Approximately(
                    enemyHealthPercent, Percent.Value);

            default:
                return false;
        }

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
