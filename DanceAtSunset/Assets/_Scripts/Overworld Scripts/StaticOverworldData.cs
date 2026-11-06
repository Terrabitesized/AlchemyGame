using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class StaticOverworldData : MonoBehaviour
{
    // Loading from Main Menu
    public static bool LoadFromMainMenu;

    public static int CurrentSaveSlot;

    // Creating a new game
    public static bool CreateNewGame;

    // Load position after a battle
    public static Vector3 PlayerPosition;
    public static Quaternion PlayerRotation;

    // Check if loading from a battle
    public static bool LoadingFromCombat = false;
}