using System;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int saveVersion = 1;

    public OverworldSaveData overworld = new OverworldSaveData();

    public CombatSaveData combat = new CombatSaveData();
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
    public Vector3Serializable playerPosition;
}

[Serializable]
public class CombatSaveData
{
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public int defense;
}