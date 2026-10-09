using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAbilityRequirement
{
    None,
    LessThanThreeEnemies
}

[CreateAssetMenu(menuName = "Combat/Enemy/Enemy Ability")]
public class EnemyAbility : ScriptableObject
{
    [Header("Effects")]
    [SerializeReference] public List<IEffectFactory<IDamagable>> effects = new();

    [Header("Targeting")]
    [SerializeReference] public EnemyAttackPattern enemyAttackPattern;

    [Header("Usage Requirements")]
    public EnemyAbilityRequirement enemyAbilityRequirement;

    [Header("Owner")]
    public Transform ownerTransform;

    public void Target(IDamagable attacker)
    {
        if (enemyAttackPattern != null)
            enemyAttackPattern.Start(this, attacker);
    }

    public void Execute(IDamagable target, IDamagable attacker)
    {
        foreach (var effect in effects)
        {
            var runtimeEffect = effect.Create();
            target.ApplyEffect(runtimeEffect, attacker);
        }
    }
}
