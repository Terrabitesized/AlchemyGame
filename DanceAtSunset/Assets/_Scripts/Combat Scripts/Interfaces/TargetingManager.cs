using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TargetingManager : MonoBehaviour
{
    TargetingStrategy currentStrategy;

    void Update()
    {
        if(currentStrategy != null && currentStrategy.IsTargeting)
        {
            currentStrategy.Update();
        }
    }

    public void SetCurrentStrategy(TargetingStrategy strategy) => currentStrategy = strategy;
    public void ClearCurrentStrategy() => currentStrategy = null;

    /// <summary>
    /// If PotionManager is not null (meaning we are in combat), returns a random IDamagable from the list of enemies in combat.
    /// </summary>
    public IDamagable GetRandomTarget()
    {
        if (CombatManager.Instance == null) return null;

        List<IDamagable> targets = CombatManager.Instance.GetEnemiesInCombat();

        Debug.Log(targets.Count + " NUMBER OF ENEMIES");

        if (targets.Count == 0) return null;

        int enemyTargetIndex = Random.Range(0, targets.Count);

        return targets[enemyTargetIndex];
    }

    /// <summary>
    /// If PotionManager is not null (meaning we are in combat), returns a list of IDamagables from the list of enemies in combat.
    /// </summary>
    public List<IDamagable> GetAllTargets()
    {
        if(CombatManager.Instance == null) return null;

        return CombatManager.Instance.GetEnemiesInCombat();
    }
}
