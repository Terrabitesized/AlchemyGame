using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class BaseEnemyAI : MonoBehaviour
{
    public bool ReducesAtkSpdWithAlliesPresent = true;
    [ShowIf(nameof(ReducesAtkSpdWithAlliesPresent))] public float AtkSpdReductionPerAllyPresent;

    public List<EnemyAbility> EnemyAbilities;
    [SerializeField] private float defaultAttackCooldown = 5f;
    private CombatManager combatManager;

    private void Start()
    {
        combatManager = CombatManager.Instance;

        StartCoroutine(SelectAttack());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator SelectAttack()
    {
        // When an enemy first spawns, wait their default cooldown
        yield return new WaitForSeconds(defaultAttackCooldown);

        EnemyAbility lastAbility = null;
        float attackSpeedModifier;
        int attackChoice = 0;
        bool validChoice = false;

        while(true)
        {
            // If we reduce our attack speed with allies present, calculate that modifier here
            attackSpeedModifier = 1f;
            if(ReducesAtkSpdWithAlliesPresent && combatManager != null)
                attackSpeedModifier = combatManager.GetEnemyCount();

            if(lastAbility != null)
            {
                if (lastAbility.enemyAttackPattern.UsesDefaultCooldown)
                    yield return new WaitForSeconds(defaultAttackCooldown * attackSpeedModifier);
                else
                    yield return new WaitForSeconds(lastAbility.enemyAttackPattern.AttackCooldown * attackSpeedModifier);
            }

            validChoice = false;
            attackChoice = 0;

            while (!validChoice)
            {
                attackChoice = Random.Range(0, EnemyAbilities.Count);

                if (lastAbility != null && !lastAbility.enemyAttackPattern.CanBeConsecutive && lastAbility == EnemyAbilities[attackChoice])
                        continue;
                else
                    validChoice = true;
            }
            

            lastAbility = EnemyAbilities[attackChoice];
            EnemyAbilities[attackChoice].Target();
        }
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
            if (!enemyAttackPattern.UsesDefaultCooldown && enemyAttackPattern.AttackCooldown <= 0)
                Debug.LogError($"EnemyAttackPattern at index {i} has an invalid AttackCooldown of {enemyAttackPattern.AttackCooldown}!");
            if (enemyAttackPattern.WarningDuration <= 0)
                Debug.LogError($"EnemyAttackPattern at index {i} has an invalid WarningDuration of {enemyAttackPattern.WarningDuration}!");
            if (enemyAttackPattern.AttackDuration <= 0)
                Debug.LogError($"EnemyAttackPattern at index {i} has an invalid AttackDuration of {enemyAttackPattern.AttackDuration}!");
        }
    }
}
