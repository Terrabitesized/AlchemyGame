using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Spells/New Spell")]
public class Spell : ScriptableObject
{
    [SerializeField] Ability spellAbility;
}
