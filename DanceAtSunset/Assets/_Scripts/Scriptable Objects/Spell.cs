using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Spells/New Spell")]
public class Spell : ScriptableObject
{
    public Ability spellAbility;
    public int spellId;
    public string spellName;
    public string spellDescription;

    public Sprite spellIcon;
    public float SpellCooldown;

    private void OnValidate()
    {
        if (SpellCooldown < .25f)
            SpellCooldown = .25f;
    }
}
