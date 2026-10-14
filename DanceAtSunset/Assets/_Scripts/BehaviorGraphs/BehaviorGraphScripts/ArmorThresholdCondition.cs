using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Armor Threshold", story: "Armor below [Percent]%", category: "Conditions", id: "c713bd8aac99d6e21d9a17b9002e4833")]
public partial class ArmorThresholdCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> Percent;

    // Enemy Stats ref
    private Stats EnemyStats;
    private float startShields;

    public override bool IsTrue()
    {
        if (EnemyStats == null || startShields <= 0f)
            return false;

        float shieldPercent =
            (EnemyStats.Shield / startShields) * 100f;

        Debug.Log(
       $"Current shield: {EnemyStats.Shield}, " +
       $"Starting shield: {startShields}, " +
       $"Remaining: {shieldPercent}%, " +
       $"Threshold: {Percent.Value}%",
       GameObject);

        return shieldPercent <= Percent.Value;
    }

    public override void OnStart()
    {
        EnemyStats = GameObject.GetComponent<EnemyStats>()?.Stats;

        if (EnemyStats != null)
            startShields = EnemyStats.Shield;

        Debug.Log(
           $"Armor condition started. Initial shield: {startShields}",
           GameObject);
    }

    public override void OnEnd()
    {
    }
}
