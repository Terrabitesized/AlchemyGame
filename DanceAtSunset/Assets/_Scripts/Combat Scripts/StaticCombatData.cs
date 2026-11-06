using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public enum CombatType
{
    Normal,
    Boss
}

public class StaticCombatData : MonoBehaviour
{
    // Enemies from Roam
    public static List<GameObject> enemies;
    public static CombatType CombatType;
    public static Tutorial TutorialType;

    // Resonance
    public static int resonanceCharge;

    public static void SetupCombat(Transform playerTransform, List<GameObject> combatEnemies, CombatType combatType, Tutorial tutorial = Tutorial.None)
    {
        // Load data based on player stats and specific enemy hit
        enemies = combatEnemies;

        CombatType = combatType;
        TutorialType = tutorial;
        StaticOverworldData.PlayerPosition = playerTransform.position;
        StaticOverworldData.PlayerRotation = playerTransform.rotation;
    }
}
