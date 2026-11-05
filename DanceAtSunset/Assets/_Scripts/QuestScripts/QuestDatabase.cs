using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public List<Quest> AllQuests;

    public Quest GetQuest(int id)
    {
        return AllQuests.Find(q => q.QuestID == id);
    }
}