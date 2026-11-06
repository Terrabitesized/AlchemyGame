using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int saveVersion = 1;

    public OverworldSaveData Overworld = new OverworldSaveData();

    public CombatSaveData Combat = new CombatSaveData();

    public QuestSaveData Quests = new QuestSaveData();
}

[Serializable]
public class OverworldSaveData
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

[Serializable]
public class CombatSaveData
{
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public int defense;
}

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