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

    // Play current stats
    public static int maxHealth;
    public static int health;
    public static int playerAttack;
    public static int playerDefense;
    public static int playerLevel;
    public static int currentExp;

    // New attempt
    public static BaseStats BaseStats;

    // Spoils
    public static int experienceEarned;

    public static void SetupCombat(GameObject player, List<GameObject> combatEnemies)
    {
        // Load data based on player stats and specific enemy hit
        message = "Balls";
        enemies = combatEnemies;

        BaseStats = player.gameObject.GetComponent<OverworldStats>().stats;

        StaticOverworldData.playerPosition = player.transform.position;
        StaticOverworldData.playerRotation = player.transform.rotation;
    }
}
