using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest UI Fields")]
    
    private List<QuestInstance> activeQuests = new List<QuestInstance>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //StartQuest(TestQuest);
    }

    /// <summary>Populates and enables the quest accept screen with the specified quest.</summary>

    public void PromptQuest(Quest quest, bool immediate = false)
    {
        if(!immediate)
        {

        }
    }

    public void StartQuest(Quest quest)
    {
        Debug.Log($"Quest started: {quest.QuestName}");

        QuestInstance instance = new QuestInstance(quest);

        instance.Initialize();

        activeQuests.Add(instance);
    }

    public void HandleEvent(QuestEvent questEvent)
    {
        foreach (QuestInstance quest in activeQuests)
        {
            quest.HandleEvent(questEvent);
        }

        CheckCompletedQuests();
    }

    private void CheckCompletedQuests()
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            QuestInstance quest = activeQuests[i];

            if (quest.IsComplete)
            {
                CompleteQuest(quest);
            }
        }
    }

    private void CompleteQuest(QuestInstance quest)
    {
        Debug.Log($"Quest completed: {quest.Definition.QuestName}");

        GiveReward(quest.Definition.Reward);

        activeQuests.Remove(quest);
    }

    private void GiveReward(QuestReward reward)
    {
        // Your player XP system here
        Debug.Log($"Awarded {reward.Experience} XP");
    }
}