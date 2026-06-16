using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CombatDialoguePopups : MonoBehaviour
{

    private void OnEnable()
    {
        PotionManager.OnAttackEnd += CheckForDefeatedEnemies;
    }

    private void OnDisable()
    {
        PotionManager.OnAttackEnd -= CheckForDefeatedEnemies;
    }

    private void CheckForDefeatedEnemies(List<GameObject> enemies)
    {
        Debug.Log("I THINK SOMEONE FUCKING DIED AHHHHHHHHHHHHHHHHHHHHH" + enemies.Count);
    }
}
