using UnityEngine;

[CreateAssetMenu(fileName = "CombatIngredient", menuName = "Scriptable Objects/CombatIngredient")]
public class CombatIngredient : ScriptableObject
{
    public string ingredientName;
    public Color color;
    public int ingredientPriority;
}
