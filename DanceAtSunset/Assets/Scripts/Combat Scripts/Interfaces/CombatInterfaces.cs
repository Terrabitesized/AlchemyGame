using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Hierarchy;
using UnityEngine;

[Serializable]
public class Ability
{
    [SerializeReference] public List<IEffect<IDamagable>> effects = new();

    public void Execute(IDamagable target)
    {
        foreach (var effect in effects)
            effect.Apply(target);
    }
}

public interface IDamagable
{
    /// <summary>
    /// Returns true if the IDamagable was killed after taking this damage.
    /// </summary>
    public bool takeDamage(int amount);
}


public interface IEffect<TTarget>
{
    void Apply(TTarget target);
    void Cancel();
    event Action<IEffect<TTarget>> OnCompleted;
}


/// <summary>
/// A single instance of damage to an enemy.
/// </summary>
[Serializable]
public class DamageEffect : IEffect<IDamagable>
{
    public int damageAmount = 10;

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

/// <summary>
/// Applies damage over time instance to an enemy.
/// </summary>
[Serializable]
public class DamageOverTimeEffect : IEffect<IDamagable>
{
    public float duration = 5f;
    public float tickInterval = 1f;
    public int damagePerTick = 1;

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