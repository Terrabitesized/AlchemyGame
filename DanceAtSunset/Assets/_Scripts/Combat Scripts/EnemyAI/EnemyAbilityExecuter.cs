using System;
using System.Collections;
using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;
using Unity.Behavior;
using UnityEngine.UIElements;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class EnemyAbilityExecuter : MonoBehaviour
{
    public bool IsChargingAbility { get; private set; }
    public static Action<GameObject, EnemyAbility> OnEnemyAbilityPrimed;

    [Header("Enemy Attack Parameters")]
    public bool ReducesAtkSpdWithAlliesPresent = true;
    [SerializeField] private GameObject abilityPopupAnimator;

    [Header("Enemy Abilities")]
    public bool HasOnSpawnAbility;
    [ShowIf(nameof(HasOnSpawnAbility))] public EnemyAbility OnSpawnAbility;
    public bool HasOnDeathAbility;
    [ShowIf(nameof(HasOnDeathAbility))] public EnemyAbility OnDeathAbility;

    private CombatManager combatManager;
    private EnemyAbility currentAbility;
    private Coroutine chargeAbilityCoroutine = null;


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

    public bool TryUseAbility(EnemyAbility ability, bool immediate = false)
    {
        if (!CombatManager.Instance.EnemiesCanAttack)
            return false;

        //Debug.Log($"{gameObject.name} just tried {ability?.name}," +
        //$"Immediate: {immediate}, " +
        //$"IsCharging: {IsChargingAbility}, ");

        if (ability == null)
            return false;

        // Don't start the same ability again while it's already charging.
        if (currentAbility == ability && IsChargingAbility)
            return false;

        // If another ability is charging, only interrupt it when Immediate is true.
        if (IsChargingAbility)
        {
            if (!immediate)
                return false;

            // Stop the current ability's charge.
            if (chargeAbilityCoroutine != null)
                StopCoroutine(chargeAbilityCoroutine);

            abilityPopupAnimator
                ?.GetComponent<AbilityPopupAnimator>()
                ?.StopPopupImmediate();

            IsChargingAbility = false;
            currentAbility = null;
            chargeAbilityCoroutine = null;
        }

        // Start charging the new ability.
        chargeAbilityCoroutine = StartCoroutine(ChargeAbility(ability));
        return true;

        // TODO CYRENE: I believe I need to add some way to tell if an ability is
        // currently *active*. It is simple enough to stop the charing of a move,
        // but if things are already spawned on field for a move, I may not have
        // an easy way of stopping that. I will at least need some way to tell if
        // a move has gameObjects spawned on field or not to determine what to do.
    }

    private IEnumerator ChargeAbility(EnemyAbility ability)
    {


        IsChargingAbility = true;
        currentAbility = ability;

        // Show the attack's warning/popup
        abilityPopupAnimator?.SetActive(true);
        abilityPopupAnimator?.GetComponent<AbilityPopupAnimator>()?.Init(
            ability.enemyAttackPattern.AttackCastTime,
            ability.enemyAttackPattern.AttackName
        );

        // Wait for the cast to finish.
        yield return GameFlowUtility.WaitForGameplaySeconds(
            ability.enemyAttackPattern.AttackCastTime
        );

        Coroutine activeAbility = StartCoroutine(ExecuteAbility(ability));

        // Small offset to allow charge UI to debuild before next ability loads
        yield return GameFlowUtility.WaitForGameplaySeconds(.5f);

        IsChargingAbility = false;

        currentAbility = null;
        chargeAbilityCoroutine = null;
        //Debug.Log($"Finished charging: {ability.name}");
    }

    private IEnumerator ExecuteAbility(EnemyAbility ability)
    {
        // Execute the ability
        ability.Target(GetComponent<IDamagable>(), gameObject);

        // Wait for the attack animation/effect to play out
        yield return GameFlowUtility.WaitForGameplaySeconds(
            CalculateAbilityDuration(ability)
        );
    }

    private float CalculateAbilityDuration(EnemyAbility enemyAbility)
    { return enemyAbility.enemyAttackPattern.WarningDuration + enemyAbility.enemyAttackPattern.AttackDuration; }
}
