using System;
using UnityEngine;

/// <summary>
/// All pertinant stat information for the player. This includes
/// combat stats, expereince, level, gold, and other numbered
/// stats. This does not include things such as inventory, or
/// equipment status (ie: does not save what items are equiped).
/// </summary>
[Serializable]
public class PlayerData
{
    [Header("Base Stats")]
    public BaseStats PlayerBaseStats;

    [Header("Leveling Fields")]
    public int CurrentLevel = 1;
    public int CurrentExperience = 0;
    public int ExperienceRequired => LevelManager.GetRequiredExperience(CurrentLevel);

    [Header("Game Statistics")]
    public int TotalPlayTime = 0;

    [Header("Events")]
    public static Action<int, LevelUpRewards> OnPlayerLevelUp;

    public PlayerData() { PlayerBaseStats = new BaseStats(1000, 1000, 5, 5); }

    public PlayerData(BaseStats baseStats, int CurrentLevel = 1, int CurrentExperience = 0, int TotalPlayTime = 0)
    {
        PlayerBaseStats = new BaseStats(baseStats);

        this.CurrentLevel = CurrentLevel;
        this.CurrentExperience = CurrentExperience;
        this.TotalPlayTime = TotalPlayTime;
    }

    public void SetHealth(int currentHealth)
    {
        PlayerBaseStats.CurrentHealth = currentHealth;
    }

    public void ModifyHealth(int amount)
    {
        PlayerBaseStats.CurrentHealth += amount;
    }

    public void ModifyAttack(int amount)
    {
        PlayerBaseStats.Attack += amount;
    }

    public void ModifyDefense(int amount)
    {
        PlayerBaseStats.Defense += amount;
    }

    public void ModifyMaxHealth(int amount)
    {
        PlayerBaseStats.MaxHealth += amount;
    }

    public void AddExperience(int experience)
    {
        CurrentExperience += experience;

        if(CurrentExperience >= ExperienceRequired)
        {
            // Level up!
            CurrentExperience -= ExperienceRequired;
            CurrentLevel++;

            // Grant rewards
            LevelUpRewards levelUpRewards = LevelManager.GetLevelUpRewards();
            PlayerBaseStats.MaxHealth += levelUpRewards.HealthGrowth;
            PlayerBaseStats.CurrentHealth += levelUpRewards.HealthGrowth;
            PlayerBaseStats.Attack += levelUpRewards.AttackGrowth;
            PlayerBaseStats.Defense += levelUpRewards.DefenseGrowth;

            OnPlayerLevelUp?.Invoke(CurrentLevel, levelUpRewards);
        }
    }

    public void AddPlayTime(int time) { TotalPlayTime += time; }
}