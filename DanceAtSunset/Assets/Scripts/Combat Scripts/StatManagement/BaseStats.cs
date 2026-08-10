using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Stats/BaseStats")]
public class BaseStats : ScriptableObject
{
    public int maxHealth;
    public int attack;
    public int defense;
    public int speed;
    public int level;
}