
using System;
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
    }
}

[Serializable]
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