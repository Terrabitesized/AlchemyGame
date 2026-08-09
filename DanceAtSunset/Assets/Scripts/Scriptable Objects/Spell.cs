using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Spells/New Spell")]
public class Spell : ScriptableObject
{
    public Ability spellAbility;
    public int spellId;
    public string spellName;
    public string spellDescription;

    public Sprite spellIcon;

    public bool hasRecipe = false;
    [ShowIf(nameof(hasRecipe))] public List<IngredientType> castingRecipe;

    private void OnValidate()
    {
        if (castingRecipe.Count > 3)
            castingRecipe.RemoveRange(3, castingRecipe.Count - 3);
    }
}
