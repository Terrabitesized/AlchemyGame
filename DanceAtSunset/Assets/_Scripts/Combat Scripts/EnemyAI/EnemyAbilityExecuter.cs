using System;
using System.Collections;
using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilityExecuter : MonoBehaviour
{
    public bool IsExecutingAbility { get; private set; }
    public static Action<GameObject, EnemyAbility> OnEnemyAbilityPrimed;

    [Header("Enemy Attack Parameters")]
    public bool ReducesAtkSpdWithAlliesPresent = true;
    [SerializeField] private GameObject abilityPopupAnimator;

    [Header("Enemy Abilities")]
    public bool HasOnSpawnAbility;
    [ShowIf(nameof(HasOnSpawnAbility))] public EnemyAbility OnSpawnAbility;
    public bool HasOnDeathAbility;
    [ShowIf(nameof(HasOnDeathAbility))] public EnemyAbility OnDeathAbility;
    public List<EnemyAbility> EnemyAbilities;

    private CombatManager combatManager;

    private EnemyAbility lastAbility = null;
    private EnemyAbility currentAbility = null;
    private float attackSpeedModifier;

    private void Start()
    {
        combatManager = CombatManager.Instance;

        abilityPopupAnimator.SetActive(false);
        abilityPopupAnimator.transform.SetParent(transform, false);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public bool TryUseAbility(EnemyAbility ability)
    {
        // TODO CYRENE: Make it so abilities can be interupted.
        // For instance, add a bool override to TryUseAbility to allow
        // a new ability to stop all others and immediately begin casting.

        if (ability == null || IsExecutingAbility)
            return false;

        StartCoroutine(ExecuteAbility(ability));
        return true;
    }

    private IEnumerator ExecuteAbility(EnemyAbility ability)
    {
        IsExecutingAbility = true;
        currentAbility = ability;

        // Show the attack's warning/popup.
        abilityPopupAnimator?.SetActive(true);
        abilityPopupAnimator?.GetComponent<AbilityPopupAnimator>()?.Init(
            ability.enemyAttackPattern.AttackCastTime,
            ability.enemyAttackPattern.AttackName
        );

        // Wait for the cast to finish.
        yield return GameFlowUtility.WaitForGameplaySeconds(
            ability.enemyAttackPattern.AttackCastTime
        );

        // Execute the ability.
        ability.Target(GetComponent<IDamagable>(), gameObject);

        // Wait for the attack animation/effect to play out.
        yield return GameFlowUtility.WaitForGameplaySeconds(
            CalculateAbilityDuration(ability)
        );

        lastAbility = ability;
        currentAbility = null;
        IsExecutingAbility = false;
    }

    private float CalculateAbilityDuration(EnemyAbility enemyAbility)
    { return enemyAbility.enemyAttackPattern.WarningDuration + enemyAbility.enemyAttackPattern.AttackDuration; }
}
