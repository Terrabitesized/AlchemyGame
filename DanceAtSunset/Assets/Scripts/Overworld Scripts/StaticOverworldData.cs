using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class StaticOverworldData : MonoBehaviour
{
    // Loading from Main Menu
    public static bool loadFromMainMenu;

    // Creating a new game
    public static bool createNewGame;

    // Load position after a battle
    public static Vector3 playerPosition;
    public static Quaternion playerRotation;

    // Check if loading from a battle
    public static bool loadingFromCombat = false;
}