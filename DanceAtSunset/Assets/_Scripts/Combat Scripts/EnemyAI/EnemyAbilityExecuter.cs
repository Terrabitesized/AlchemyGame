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
    private List<Coroutine> activeAbilities = new List<Coroutine>();


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
        Debug.Log(
    $"TryUseAbility: {ability?.name}, " +
    $"Immediate: {immediate}, " +
    $"IsCharging: {IsChargingAbility}, " +
    $"Current: {currentAbility?.name}"
);

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


        //// Check if the abiity we were passed should be used immediately. If so,
        //// and we are currently using an ability, we need to stop it here
        //if (immediate && chargeAbilityCoroutine != null)
        //{
        //    // TODO CYRENE: I believe I need to add some way to tell if an ability is
        //    // currently *active*. It is simple enough to stop the charing of a move,
        //    // but if things are already spawned on field for a move, I may not have
        //    // an easy way of stopping that. I will at least need some way to tell if
        //    // a move has gameObjects spawned on field or not to determine what to do.

        //    // Stop all operations related the current ability, and start this one
        //    StopCoroutine(chargeAbilityCoroutine);
        //    abilityPopupAnimator?.GetComponent<AbilityPopupAnimator>().StopPopupImmediate();
        //    IsChargingAbility = false;
        //}

        //// If we get passed and ability and immediate == false, we stop here
        //if (ability == null || IsChargingAbility)
        //{
        //    Debug.LogError($"ability == null || IsChargingAbility {IsChargingAbility}");
        //    return false;
        //}

        //// Start performing the ability
        //chargeAbilityCoroutine = StartCoroutine(ChargeAbility(ability));
        //return true;
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
        IsChargingAbility = false;

        currentAbility = null;
        chargeAbilityCoroutine = null;
        Debug.Log($"Finished charging: {ability.name}");
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

    private IEnumerator ExecuteAndTrackAbility(EnemyAbility ability)
    {
        Coroutine handle = null;

        // Start the actual ability as a separate coroutine
        handle = StartCoroutine(ExecuteAbility(ability));

        activeAbilities.Add(handle);

        // Wait until the actual ability finishes
        yield return handle;

        // Remove the completed coroutine
        activeAbilities.Remove(handle);
    }

    private float CalculateAbilityDuration(EnemyAbility enemyAbility)
    { return enemyAbility.enemyAttackPattern.WarningDuration + enemyAbility.enemyAttackPattern.AttackDuration; }
}
