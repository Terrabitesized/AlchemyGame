using System.Collections;
using UnityEngine;

public abstract class Ultimate : ScriptableObject
{
    public Ability ultimateAbility;

    public abstract IEnumerator PlayUltimate(CombatManager combatManager);
}