using System.Collections;
using Alchemy.Inspector;
using UnityEngine;

public abstract class Ultimate : ScriptableObject
{
    public Ability UltimateAbility;
    public bool HideMap;
    [ShowIf(nameof(HideMap))] public GameObject CutsceneMap;

    public abstract IEnumerator PlayUltimate(CombatManager combatManager);
}