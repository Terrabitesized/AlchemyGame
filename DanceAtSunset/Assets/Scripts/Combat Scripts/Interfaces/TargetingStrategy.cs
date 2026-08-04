
using System;

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