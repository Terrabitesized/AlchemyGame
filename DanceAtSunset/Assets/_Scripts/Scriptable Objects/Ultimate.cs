using System.Collections;
using Alchemy.Inspector;
using UnityEngine;

public abstract class Ultimate : ScriptableObject
{
    public Ability UltimateAbility;
    public bool HideMap;
    [ShowIf(nameof(HideMap))] public GameObject CutsceneMap;
    public GameObject CutsceneResources;
    public Animator CutsceneAnimator;

    public abstract IEnumerator PlayUltimate(CombatManager combatManager);
    public void SetCutsceneAnimator(Animator animator)
    {
        CutsceneAnimator = animator;
    }
}