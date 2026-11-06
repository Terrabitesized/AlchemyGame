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
    public int maxHealth;
    public int currentHealth;
    public int shield;
    public int attack;
    public int defense;
    public int speed;
    public int level;

    public BaseStats(BaseStats stats)
    {
        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;
        shield = stats.shield;
        attack = stats.attack;
        defense = stats.defense;
        speed = stats.speed;
        level = stats.level;
    }
}