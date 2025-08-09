using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class StaticCombatData : MonoBehaviour
{
    // Enemies from Roam
    public static string message;
    public static List<GameObject> enemies;

    // Play current stats
    public static int playerAttack;
    public static int playerDefense;
    public static int maxHealth;
    public static int playerLevel;
    public static int currentExp;

    // Spoils
    public static int experienceEarned;
}
