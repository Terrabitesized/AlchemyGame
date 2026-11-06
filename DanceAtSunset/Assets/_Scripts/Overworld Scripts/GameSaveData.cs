using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int saveVersion = 1;

    public PlayerStatSaveData PlayerStats = new PlayerStatSaveData();

    //public CombatSaveData Combat = new CombatSaveData();

    public QuestSaveData Quests = new QuestSaveData();
}

/**
 * Any data relevant to the playerStats.
 */

[Serializable]
public class PlayerStatSaveData
{
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public int defense;
    public int speed;
    public int level;

    public int exp;
    public int maxExp;

    public float totalPlayTime;

    public string sceneName;
    public Vector3Serializable PlayerPosition;
}

//[Serializable]
//public class CombatSaveData
//{
//    public int maxHealth;
//    public int currentHealth;
//    public int attack;
//    public int defense;
//}

[Serializable]
public class QuestSaveData
{
    public List<QuestInstanceSaveData> ActiveQuests = new List<QuestInstanceSaveData>();
    public List<int> CompletedQuests = new List<int>();
}

[Serializable]
public class QuestInstanceSaveData
{
    public int QuestID;
    public List<int> ObjectiveProgress = new();
}