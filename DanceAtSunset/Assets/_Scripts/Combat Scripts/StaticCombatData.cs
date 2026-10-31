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
    public static string message;
    public static List<GameObject> enemies;
    public static CombatType CombatType;
    public static Tutorial TutorialType;

    // Player Stats
    public static BaseStats BaseStats;
    public static int currentExp;

    // Resonance
    public static int resonanceCharge;

    // Spoils
    public static int experienceEarned;

    public static void SetupCombat(GameObject player, List<GameObject> combatEnemies, CombatType combatType, Tutorial tutorial = Tutorial.None)
    {
        // Load data based on player stats and specific enemy hit
        message = "Balls";
        enemies = combatEnemies;

        BaseStats = player.gameObject.GetComponent<OverworldStats>().stats;

        CombatType = combatType;
        TutorialType = tutorial;
        StaticOverworldData.playerPosition = player.transform.position;
        StaticOverworldData.playerRotation = player.transform.rotation;
    }
}
