using UnityEngine;

[CreateAssetMenu(fileName = "CombatCrystal", menuName = "Scriptable Objects/CombatCrystal")]
public class CombatCrystal : ScriptableObject
{
    public string ingredientName;
    public Color color;
    public int ingredientPriority;
    public Spell crystalSpell;
}
