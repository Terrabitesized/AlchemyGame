using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class StaticCombatData : MonoBehaviour
{
    // Enemies from Roam
    public static string message;
    public static List<GameObject> enemies;

    // Play current stats
    public static int maxHealth;
    public static int playerAttack;
    public static int playerDefense;
    public static int playerLevel;
    public static int currentExp;

    // Spoils
    public static int experienceEarned;

    public static void SetupCombat(GameObject player, List<GameObject> combatEnemies)
    {
        // Load data based on player stats and specific enemy hit
        message = "Balls";
        enemies = combatEnemies;

        playerAttack = player.gameObject.GetComponent<OverworldStats>().getAtk();
        playerDefense = player.gameObject.GetComponent<OverworldStats>().getDef();
        playerLevel = player.gameObject.GetComponent<OverworldStats>().getLevel();
        maxHealth = player.gameObject.GetComponent<OverworldStats>().getMaxHp();
        currentExp = player.gameObject.GetComponent<OverworldStats>().getExp();

        StaticOverworldData.playerPosition = player.transform.position;
        StaticOverworldData.playerRotation = player.transform.rotation;
    }
}
