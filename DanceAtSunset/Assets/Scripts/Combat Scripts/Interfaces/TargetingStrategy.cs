
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetingStrategy
{
    protected Ability abilty;
    protected TargetingManager targetingManager;
    protected bool isTargeting = false;
    public bool IsTargeting => isTargeting;

    public abstract void Start(Ability ability, TargetingManager targetingManager);
    public virtual void Update() { }
    public virtual void Cancel() { }
}

[Serializable]
public class SelfTargeting : TargetingStrategy
{
    public override void Start(Ability ability, TargetingManager targetingManager)
    {
        this.abilty = ability;
        this.targetingManager = targetingManager;

        if(targetingManager.transform.TryGetComponent<IDamagable>(out var target))
        {
            ability.Execute(target);
        }

        ability.AbilityCompletion(targetingManager);
    }
}

[Serializable]
public class SingleTargeting : TargetingStrategy
{
    public override void Start(Ability ability, TargetingManager targetingManager)
    {
        this.abilty = ability;
        this.targetingManager = targetingManager;

        IDamagable target = targetingManager.GetRandomTarget();

        if (target != null)
        {
            ability.Execute(target);
        } else
        {
            Debug.Log("I COULDN'T FIND A TARGET");
        }

        ability.AbilityCompletion(targetingManager);
    }
}

[Serializable]
public class AllTargeting : TargetingStrategy
{
    public override void Start(Ability ability, TargetingManager targetingManager)
    {
        this.abilty = ability;
        this.targetingManager = targetingManager;

        List<IDamagable> targets = targetingManager.GetAllTargets();

        foreach (IDamagable t in targets)
        {
            if(t != null)
                ability.Execute(t);
        }

        ability.AbilityCompletion(targetingManager);
    }
}

[Serializable]
public class BounceTargeting : TargetingStrategy
{
    public int bounces = 1;
    public float bounceInterval = .1f;

    public override void Start(Ability ability, TargetingManager targetingManager)
    {
        this.abilty = ability;
        this.targetingManager = targetingManager;

        CoroutineRunner.Instance?.StartCoroutine(BounceCoroutine(ability));
    }

    public IEnumerator BounceCoroutine(Ability ability)
    {
        for (int i = 0; i < this.bounces; i++)
        {
            IDamagable target = targetingManager.GetRandomTarget();

            if (target != null)
            {
                ability.Execute(target);
            }
            else
            {
                Debug.Log("I COULDN'T FIND A TARGET");
                break;
            }

            yield return new WaitForSeconds(bounceInterval);
        }

        ability.AbilityCompletion(targetingManager);
    }
}

// UNUSED VVVV
public class AOETargeting : TargetingStrategy
{
    public GameObject aoePrefab;
    public float aoeRadius = 5f;
    public LayerMask groundLayerMask = 1;

    GameObject previewInstance;

    public override void Start(Ability ability, TargetingManager targetingManager)
    {
        this.abilty = ability;
        this.targetingManager = targetingManager;
        isTargeting = true;

        targetingManager.SetCurrentStrategy(this);

        if (aoePrefab != null)
        {
            previewInstance = UnityEngine.Object.Instantiate(aoePrefab, Vector3.zero + new Vector3(0f, .1f, 0f), Quaternion.identity);
        }
    }
}