using System;
using System.Collections.Generic;
using UnityEngine;



public class QuestManager : MonoBehaviour, ISavable
{
    public static QuestManager Instance;

    public static Action<QuestInstance> OnQuestStarted;
    public static Action<QuestInstance> OnQuestCompleted;
    public static Action<QuestInstance> OnQuestFailed;

    [SerializeField] private QuestDatabase questDatabase;
    [SerializeField] private List<QuestInstance> activeQuests = new List<QuestInstance>();
    [SerializeField] private List<Quest> completedQuests = new List<Quest>();

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

    public void CheckQuestCompletion(QuestInstance quest)
    {
        if (quest.IsComplete && quest.Definition.ManualCompletion)
        {
            CompleteQuest(quest);
        }
    }

    private void CheckCompletedQuests()
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            QuestInstance quest = activeQuests[i];

            if (quest.IsComplete && !quest.Definition.ManualCompletion)
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
        completedQuests.Add(quest.Definition);

        OnQuestCompleted?.Invoke(quest);
    }

    private void GiveReward(QuestReward reward)
    {
        // Your player XP system here
        Debug.Log($"Awarded {reward.Experience} XP");
        PlayerSession.PlayerData.AddExperience(reward.Experience);
        PlayerSession.PlayerData.ModifyGold(reward.Gold);
    }

    public List<QuestInstance> GetActiveQuests() { return activeQuests; }

    public List<Quest> GetCompletedQuests() { return completedQuests; }

    public QuestInstance GetQuestInstanceFromQuestID(int questID)
    {
        foreach(QuestInstance questInstance in activeQuests)
        {
            if(questInstance.Definition.QuestID == questID)
                return questInstance;
        }

        return null;
    }

    public void CaptureSaveData(GameSaveData data)
    {
        data.Quests.ActiveQuests.Clear();
        data.Quests.CompletedQuests.Clear();

        foreach (QuestInstance quest in activeQuests)
        {
            data.Quests.ActiveQuests.Add(quest.GetSaveData());
        }

        foreach (Quest quest in completedQuests)
        {
            data.Quests.CompletedQuests.Add(quest.QuestID);
        }
    }

    public void RestoreSaveData(GameSaveData data)
    {
        activeQuests.Clear();
        completedQuests.Clear();

        foreach (QuestInstanceSaveData questData in data.Quests.ActiveQuests)
        {
            Quest definition = questDatabase.GetQuest(questData.QuestID);

            if (definition == null)
            {
                Debug.LogWarning($"Could not find Quest ID: {questData.QuestID}");
                continue;
            }

            QuestInstance instance = new QuestInstance(definition);

            instance.LoadSaveData(questData);

            activeQuests.Add(instance);
        }

        foreach (int questID in data.Quests.CompletedQuests)
        {
            Quest quest = questDatabase.GetQuest(questID);

            if (quest != null)
                completedQuests.Add(quest);
        }

        QuestListCanvas.Instance?.LoadAllQuests();
    }
}