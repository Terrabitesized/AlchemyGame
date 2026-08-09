using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using NUnit.Framework;
using Unity.Hierarchy;
using UnityEngine;

// GENERAL STRUCTURE:
/**
 * All things in the game with health should implement IDamagable. Each 'skill' in the game
 * is an Ability, which contains a list of specific effects. An ability can have multiple effects,
 * and applies all of them to a given IDamagable target based on a chosen targeting strategy.
 * 
 * In essence -> Abilities are created, which have a list of effects. Then, when an ability is used,
 * the ability determines who to target based on its pre-determined targeting strategy. Once a target
 * that implements IDamagable is found, it then calls Ability.Execute(), which calls Effect.Apply()
 * for each effect on the given ability.
 * 
 * https://www.youtube.com/watch?v=aZanRrhBg-8
 */


[Serializable]
public class Ability
{
    [Header("Effects")]
    [SerializeReference] public List<IEffectFactory<IDamagable>> effects = new();

    [Header("Targeting")]
    [SerializeReference] TargetingStrategy targetingStrategy;

    [Header("Casting Properties")]
    [SerializeReference] public bool requiresCasting;
    [ShowIf(nameof(requiresCasting))] public float castDuration;

    [SerializeReference] public Ability followUpAbility = null;
    private bool hasCompleted = false;

    public void Target(TargetingManager targetingManager)
    {
        if(targetingStrategy != null)
            targetingStrategy.Start(this, targetingManager);
    }

    public void Execute(IDamagable target)
    {
        foreach (var effect in effects)
        {
            var runtimeEffect = effect.Create();
            target.ApplyEffect(runtimeEffect);
        }
    }

    // Called after a given targeting strategy finishes, allowing for a single ability
    // to have multiple effects with different targeting strategies.
    public void AbilityCompletion(TargetingManager targetingManager)
    {
        if (followUpAbility != null)
        {
            followUpAbility.Target(targetingManager);
        }
    }
}

public interface IDamagable
{
    /// <summary>
    /// Returns true if the IDamagable was killed after taking this damage.
    /// </summary>
    Stats Stats { get; set; }
    bool takeDamage(int amount);
    void ApplyEffect(IEffect<IDamagable> effect);
}

public interface IEffectFactory<TTarget>
{
    IEffect<TTarget> Create();
}

public interface IEffect<TTarget>
{
    void Apply(TTarget target);
    void Cancel();
    event Action<IEffect<TTarget>> OnCompleted;
}

[Serializable]
public class DamageEffectFactory : IEffectFactory<IDamagable>
{
    public int damageAmount = 10;

    public IEffect<IDamagable> Create()
    {
        return new DamageEffect { damageAmount = damageAmount };

    }

}

/// <summary>
/// A single instance of damage to an enemy.
/// </summary>
[Serializable]
public struct DamageEffect : IEffect<IDamagable>
{
    public int damageAmount;

    public event Action<IEffect<IDamagable>> OnCompleted;

    public void Apply(IDamagable target)
    {
        target.takeDamage(damageAmount);
        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}

[Serializable]
public class StatModifyingEffectFactory : IEffectFactory<IDamagable>
{
    public StatType statType = StatType.Attack;
    public OperatorType operatorType = OperatorType.Add;
    public int value = 5;
    public float duration = 10f;

    public IEffect<IDamagable> Create()
    {
        return new StatModifyingEffect
        {
            statType = statType,
            operatorType = operatorType,
            value = value,
            duration = duration
        };


    }

}

/// <summary>
/// Modifies a specified stat for a set duration using the given value and operation.
/// </summary>
[Serializable]
public struct StatModifyingEffect : IEffect<IDamagable>
{
    public StatType statType;
    public OperatorType operatorType;
    public int value;
    public float duration;

    public event Action<IEffect<IDamagable>> OnCompleted;

    public void Apply(IDamagable target)
    {
        int modifierValue = value;

        target.Stats.Mediator.AddModifier(new BasicStatModifier(
            statType,
            duration,
            v => v + modifierValue));
        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}

[Serializable]
public class DamageOverTimeEffectFactory : IEffectFactory<IDamagable>
{
    public float duration = 5f;
    public float tickInterval = 1f;
    public int damagePerTick = 1;

    public IEffect<IDamagable> Create()
    {
        return new DamageOverTimeEffect
        { 
            duration = duration,
            tickInterval = tickInterval,
            damagePerTick = damagePerTick
        };

    }

}

/// <summary>
/// Applies damage over time instance to an enemy.
/// </summary>
[Serializable]
public struct DamageOverTimeEffect : IEffect<IDamagable>
{
    public float duration;
    public float tickInterval;
    public int damagePerTick;

    IDamagable target;
    Coroutine damageCoroutine;

    public event Action<IEffect<IDamagable>> OnCompleted;

    public void Apply(IDamagable target)
    {
        this.target = target;

        damageCoroutine = CoroutineRunner.Instance?.StartCoroutine(DamageTick());
    }

    public IEnumerator DamageTick()
    {
        Debug.Log("Starting my damage coroutine");

        for(float i = 0; i < duration; i += tickInterval)
        {
            Debug.Log("SHOULD BE DEALINGD DAMGEAGE");
            yield return new WaitForSeconds(tickInterval);
            target?.takeDamage(damagePerTick);
        }

        Cancel();
    }

    public void Cancel()
    {
        CoroutineRunner.Instance?.StopCoroutine(damageCoroutine);

        target = null;
        damageCoroutine = null;
        OnCompleted?.Invoke(this);
    }
}