using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public static Action<QuestInstance> OnQuestStarted;
    public static Action<QuestInstance> OnQuestCompleted;
    public static Action<QuestInstance> OnQuestFailed;

    [SerializeField] private List<QuestInstance> activeQuests = new List<QuestInstance>();

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

    /// <summary>Populates and enables the quest accept screen with the specified quest.</summary>

    public void PromptQuest(Quest quest, DialogueManager manager, int acceptIndex, int declineIndex, bool immediate = false)
    {
        if(immediate)
            StartQuest(quest);
        else
            QuestPromptCanvas.Instance?.PromptQuest(quest, manager, acceptIndex, declineIndex);
    }

    public void StartQuest(Quest quest)
    {
        Debug.Log($"Quest started: {quest.QuestName}");

        QuestInstance instance = new QuestInstance(quest);

        instance.Initialize();

        activeQuests.Add(instance);

        OnQuestStarted?.Invoke(instance);
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

        OnQuestCompleted?.Invoke(quest);
    }

    private void GiveReward(QuestReward reward)
    {
        // Your player XP system here
        Debug.Log($"Awarded {reward.Experience} XP");
    }
}