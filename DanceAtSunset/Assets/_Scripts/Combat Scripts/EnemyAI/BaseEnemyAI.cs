using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(EnemyStats))]
public class BaseEnemyAI : MonoBehaviour
{
    public static Action<GameObject, EnemyAbility> OnEnemyAbilityPrimed;

    public bool ReducesAtkSpdWithAlliesPresent = true;
    public List<EnemyAbility> EnemyAbilities;
    [SerializeField] private float attackCooldown = 5f;

    private CombatManager combatManager;
    [SerializeField] private AbilityPopupAnimator abilityPopupAnimator;

    private EnemyAbility lastAbility = null;
    private EnemyAbility currentAbility = null;
    private float attackSpeedModifier;

    private void Start()
    {
        combatManager = CombatManager.Instance;

        StartCoroutine(Attack());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator Attack()
    {
        // When an enemy first spawns, wait the attack cooldown
        yield return new WaitForSeconds(attackCooldown * UnityEngine.Random.Range(.8f, 1.2f));

        while(true)
        {
            CalculateAttackSpeedModifier();

            // Cooldown in between attacks. If we just spawned (lastAbility == null), skip this step.
            if (lastAbility != null)
                yield return new WaitForSeconds(attackCooldown * attackSpeedModifier);

            // Select a valid ability
            currentAbility = SelectValidAbility();

            // Enable and Init popup
            abilityPopupAnimator?.gameObject.SetActive(true);
            abilityPopupAnimator?.Init(currentAbility.enemyAttackPattern.AttackCastTime,
                currentAbility.enemyAttackPattern.AttackName);
            yield return new WaitForSeconds(currentAbility.enemyAttackPattern.AttackCastTime);

            // Attack
            currentAbility.Target(GetComponent<IDamagable>());

            // Wait for attack to play out
            yield return new WaitForSeconds(CalculateAbilityDuration(currentAbility));

            // Update lastAbility
            lastAbility = currentAbility;
        }
    }

    private EnemyAbility SelectValidAbility()
    {
        EnemyAbility validAbility = null;

        int attackChoice = 0;
        bool validChoice = false;

        // Determine which attack to use. If the last used attack cannot be consecutive, reroll
        while (!validChoice)
        {
            validAbility = EnemyAbilities[UnityEngine.Random.Range(0, EnemyAbilities.Count)];

            if (!CheckAbilityRequirements(validAbility))
                continue;

            if (lastAbility != null && !lastAbility.enemyAttackPattern.CanBeConsecutive && lastAbility == EnemyAbilities[attackChoice])
                continue;
            else
                validChoice = true;
        }

        return validAbility;
    }

    private bool CheckAbilityRequirements(EnemyAbility ability)
    {
        switch (ability.enemyAbilityRequirement)
        {
            case EnemyAbilityRequirement.None:
                return true;

            case EnemyAbilityRequirement.LessThanThreeEnemies:
                return combatManager.GetEnemyCount() < 3;

            default:
                return true;
        }
    }

    private float CalculateAbilityDuration(EnemyAbility enemyAbility) 
    { return enemyAbility.enemyAttackPattern.WarningDuration + enemyAbility.enemyAttackPattern.AttackDuration; }

    private void CalculateAttackSpeedModifier()
    {
        // If we reduce our attack speed with allies present, calculate that modifier here
        attackSpeedModifier = 1f;
        if (ReducesAtkSpdWithAlliesPresent && combatManager != null)
            attackSpeedModifier = combatManager.GetEnemyCount();
    }
    private void OnValidate()
    {
        // Debug an error if any enemy attacks have invalid values
        for (int i = 0; i < EnemyAbilities.Count; i++)
        {
            EnemyAttackPattern enemyAttackPattern = EnemyAbilities[i].enemyAttackPattern;

            if (enemyAttackPattern.AttackName.Length <= 0)
                Debug.LogError($"EnemyAttackPattern at index {i} has no name!");
            if(enemyAttackPattern.AttackCastTime <= 0)
                Debug.LogError($"EnemyAttackPattern at index {i} has an invalid AttackCastTime of {enemyAttackPattern.AttackCastTime}!");
            if (enemyAttackPattern.WarningDuration <= 0)
                Debug.LogError($"EnemyAttackPattern at index {i} has an invalid WarningDuration of {enemyAttackPattern.WarningDuration}!");
            if (enemyAttackPattern.AttackDuration <= 0)
                Debug.LogError($"EnemyAttackPattern at index {i} has an invalid AttackDuration of {enemyAttackPattern.AttackDuration}!");
        }
    }
}
