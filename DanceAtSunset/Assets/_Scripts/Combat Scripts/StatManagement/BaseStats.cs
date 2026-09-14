using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Stats/BaseStats")]
public class BaseStats : ScriptableObject
{
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public int defense;
    public int speed;
    public int level;

    public BaseStats(BaseStats stats)
    {
        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;
        attack = stats.attack;
        defense = stats.defense;
        speed = stats.speed;
        level = stats.level;
    }
}