using UnityEngine;

[CreateAssetMenu(fileName = "CombatICrystal", menuName = "Scriptable Objects/CombatICrystal")]
public class CombatICrystal : ScriptableObject
{
    public string ingredientName;
    public Color color;
    public int ingredientPriority;
    public Spell ingredientSpell;
}
