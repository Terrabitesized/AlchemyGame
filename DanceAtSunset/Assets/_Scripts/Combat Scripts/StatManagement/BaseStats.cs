using UnityEngine;

/// <summary>
/// Used by both players and enemies. Contains only combat relevant stats,
/// and can only be written to from the overworld. The player’s BaseStats
/// can be modified with equippables and by leveling up. This class should
/// not be written to at any point during combat, unless leveling up at the
/// end of a battle.
/// </summary>
[CreateAssetMenu(fileName = "BaseStats", menuName = "Stats/BaseStats")]
public class BaseStats : ScriptableObject
{
    public int MaxHealth;
    public int CurrentHealth;
    public int Attack;
    public int Defense;

    public BaseStats(BaseStats stats)
    {
        MaxHealth = stats.MaxHealth;
        CurrentHealth = stats.CurrentHealth;
        Attack = stats.Attack;
        Defense = stats.Defense;
    }

    public BaseStats(int maxHealth, int currentHealth, int attack, int defense)
    {
        this.MaxHealth = maxHealth;
        this.CurrentHealth = currentHealth;
        this.Attack = attack;
        this.Defense = defense;
    }
}