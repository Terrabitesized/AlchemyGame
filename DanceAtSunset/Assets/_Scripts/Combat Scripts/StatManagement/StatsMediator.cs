using System;
using System.Collections.Generic;

public class StatsMediator
{
    readonly List<StatModifier> modifiers = new();

    public event EventHandler<Query> Queries;
    public void PerformQuery(object sender, Query query) => Queries?.Invoke(sender, query);

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        Queries += modifier.Handle;

        modifier.OnDispose += _ =>
        {
            modifiers.Remove(modifier);
            Queries -= modifier.Handle;
        };
    }

    public void Update(float deltaTime)
    {
        // Sort the modifiers here?

        // Update all modifiers currently active, ticking them down if applicable
        foreach (StatModifier modifier in modifiers)
        {
            if(modifier != null)
                modifier.Update(deltaTime);
        }

        // If any modifier's counterdown has expired, dispose of it
        for (int i = modifiers.Count; i > 0; i--)
        {
            var modifier = modifiers[i - 1];

            if (modifier != null && modifier.MarkedForRemoval)
            {
                modifier.Dispose();
            }
        }
    }
}

public class Query
{
    public readonly StatType StatType;
    public int Value;

    public Query(StatType statType, int value)
    {
        StatType = statType;
        Value = value;
    }
}

