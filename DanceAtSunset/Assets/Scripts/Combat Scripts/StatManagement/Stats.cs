using UnityEngine;

public enum StatType { MaxHealth, Health, Attack, Defense, Level }
public enum OperatorType { Add, Multiply }

public class Stats
{
    readonly StatsMediator mediator;
    readonly BaseStats baseStats;

    public StatsMediator Mediator => mediator;

    public int MaxHealth
    {
        get
        {
            var q = new Query(StatType.MaxHealth, baseStats.maxHealth);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public int Attack
    {
        get
        {
            var q = new Query(StatType.Attack, baseStats.attack);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public int Defense
    {
        get
        {
            var q = new Query(StatType.Defense, baseStats.defense);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public int Level
    {
        get
        {
            var q = new Query(StatType.Level, baseStats.level);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public Stats(StatsMediator mediator, BaseStats baseStats)
    {
        this.mediator = mediator;
        this.baseStats = baseStats;
    }

    public override string ToString() => $"Max Health: {MaxHealth}, Attack: {Attack}, Defense: {Defense}, Level: {Level}";
}
