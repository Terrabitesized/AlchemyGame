using System;
using Unity.Behavior;
using UnityEngine;

public enum HealthComparison
{
    LessThan,
    GreaterThan,
    Equal
}

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player Health Threshold", story: "Player health [Comparison] [Percent]%", category: "Conditions", id: "c85f6b48ac5a94341691c8c2c268fca8")]
public partial class PlayerHealthBelowCondition : Condition
{
    [SerializeReference]
    public BlackboardVariable<float> Percent;

    [SerializeReference]
    public BlackboardVariable<HealthComparison> Comparison;

    private PlayerStats playerStats;

    public override void OnStart()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerStats = player != null
            ? player.GetComponent<PlayerStats>()
            : null;
    }

    public override bool IsTrue()
    {
        if (playerStats == null || playerStats.Stats == null)
            return false;

        float maxHealth = playerStats.Stats.MaxHealth;

        if (maxHealth <= 0f)
            return false;

        float healthPercent =
            (float)playerStats.Stats.CurrentHealth / maxHealth * 100f;

        switch (Comparison.Value)
        {
            case HealthComparison.LessThan:
                return healthPercent < Percent.Value;

            case HealthComparison.GreaterThan:
                return healthPercent > Percent.Value;

            case HealthComparison.Equal:
                return Mathf.Approximately(
                    healthPercent, Percent.Value);

            default:
                return false;
        }
    }

    public override void OnEnd()
    {
    }
}