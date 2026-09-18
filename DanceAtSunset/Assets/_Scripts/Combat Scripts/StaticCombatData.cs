using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public enum CombatType
{
    Tutorial,
    Normal,
    Boss
}

public class StaticCombatData : MonoBehaviour
{
    // Enemies from Roam
    public static string message;
    public static List<GameObject> enemies;
    public static CombatType CombatType;

    // Player Stats
    public static BaseStats BaseStats;
    public static int currentExp;

    // Resonance
    public static int resonanceCharge;

    // Spoils
    public static int experienceEarned;

    public static void SetupCombat(GameObject player, List<GameObject> combatEnemies, CombatType combatType)
    {
        // Load data based on player stats and specific enemy hit
        message = "Balls";
        enemies = combatEnemies;

        BaseStats = player.gameObject.GetComponent<OverworldStats>().stats;

        CombatType = combatType;
        StaticOverworldData.playerPosition = player.transform.position;
        StaticOverworldData.playerRotation = player.transform.rotation;
    }
}
