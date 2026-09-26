using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string QuestName;

    [TextArea]
    public string Description;

    [SerializeReference]
    public List<QuestObjective> Objectives = new();

    public bool ManualCompletion = false;
    public QuestReward Reward;
}

[Serializable]
public class QuestReward
{
    public int Experience;
}