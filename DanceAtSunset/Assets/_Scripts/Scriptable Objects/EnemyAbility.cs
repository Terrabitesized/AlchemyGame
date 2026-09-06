using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyAbility
{
    [Header("Effects")]
    [SerializeReference] public List<IEffectFactory<IDamagable>> effects = new();

    [Header("Targeting")]
    [SerializeReference] public EnemyAttackPattern enemyAttackPattern;

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
