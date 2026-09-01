using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyAbility
{
    [Header("Effects")]
    [SerializeReference] public List<IEffectFactory<IDamagable>> effects = new();

    [Header("Targeting")]
    [SerializeReference] EnemyAttackPattern enemyAttackPattern;

    [Header("Owner")]
    public Transform ownerTransform;

    public void Target()
    {
        if (enemyAttackPattern != null)
            enemyAttackPattern.Start(this);
    }

    public void Execute(IDamagable target)
    {
        foreach (var effect in effects)
        {
            var runtimeEffect = effect.Create();
            target.ApplyEffect(runtimeEffect);
        }
    }
}
