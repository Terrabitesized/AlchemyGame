using Random = UnityEngine.Random;
using System;
using UnityEngine;

/// <summary>
/// Used to determine all things dealing with the player leveling up.
/// Including calculating experience to next level, and stat improvements
/// on level up.
/// </summary>
public static class LevelManager
{
    #region STAT_INCREASE_FIELDS
    private const int minimumHealthOnLevelUp = 5;
    private const int maximumHealthOnLevelUp = 15;

    private const int minimumAttackOnLevelUp = 1;
    private const int maximumAttackOnLevelUp = 2;

    private const int minimumDefenseOnLevelUp = 1;
    private const int maximumDefenseOnLevelUp = 2;
    #endregion

    #region LEVEL_FORMULA_FIELDS
    private const int baseExperience = 100;
    private readonly static float growthRate = 1.25f;
    #endregion

    public static int GetRequiredExperience(int currentLevel)
    {
        return Mathf.RoundToInt(baseExperience * Mathf.Pow(currentLevel, growthRate));
    }

    public static LevelUpRewards GetLevelUpRewards()
    {
        return new LevelUpRewards
        {
            HealthGrowth = Random.Range(minimumHealthOnLevelUp, maximumHealthOnLevelUp + 1),
            AttackGrowth = Random.Range(minimumAttackOnLevelUp, maximumAttackOnLevelUp + 1),
            DefenseGrowth = Random.Range(minimumDefenseOnLevelUp, maximumDefenseOnLevelUp + 1)
        };
    }
}

/// <summary>
/// Fired as part of an event when a level up occurs. Used to
/// communicate how the player's BaseStats should increase when
/// leveling up.
/// </summary>
[Serializable]
public class LevelUpRewards
{
    public int HealthGrowth;
    public int AttackGrowth;
    public int DefenseGrowth;
}