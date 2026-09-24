using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    //[Header("Quest UI Fields")]
    //[SerializeField] private CanvasGroup questCanvasGroup;
    //[SerializeField] private TextMeshProUGUI questNameText;
    //[SerializeField] private TextMeshProUGUI questDescriptionText;
    //[SerializeField] private TextMeshProUGUI questObjectivesText;
    //[SerializeField] private TextMeshProUGUI questRewardsText;
    //[SerializeField] private Button questAcceptButton;
    //[SerializeField] private Button questDeclineButton;

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
        //if(!immediate)
        //{
        //    // Setup screen
        //    PopulateQuestInfo(quest);
        //    questCanvasGroup.alpha = 1f;

        //    // Setup buttons
        //    questAcceptButton.onClick.AddListener(() =>
        //    {
        //        StartQuest(quest);
        //        manager.ContinueDialogue(acceptIndex);
        //        manager.ToggleDialogueVisiblity(true);
        //        manager.ToggleAdvanceInput(true);

        //        questCanvasGroup.alpha = 0f;

        //        questAcceptButton.onClick.RemoveAllListeners();
        //        questDeclineButton.onClick.RemoveAllListeners();
        //    });

        //    questDeclineButton.onClick.AddListener(() =>
        //    {
        //        manager.ContinueDialogue(declineIndex);
        //        manager.ToggleDialogueVisiblity(true);
        //        manager.ToggleAdvanceInput(true);

        //        questCanvasGroup.alpha = 0f;

        //        questAcceptButton.onClick.RemoveAllListeners();
        //        questDeclineButton.onClick.RemoveAllListeners();
        //    });

        //    EventSystem.current.SetSelectedGameObject(questAcceptButton.gameObject);
        //}

        if(immediate)
            StartQuest(quest);
        else
            QuestCanvas.Instance?.PromptQuest(quest, manager, acceptIndex, declineIndex);
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

    //private void PopulateQuestInfo(Quest quest)
    //{
    //    questNameText.text = quest.QuestName;
    //    questDescriptionText.text = quest.Description;

    //    string questObjectivesTextList = "";
    //    for(int i = 0; i < quest.Objectives.Count; i++)
    //    {
    //        questObjectivesTextList += quest.Objectives[i].ToString();

    //        if(i < quest.Objectives.Count - 1)
    //            questObjectivesTextList += "\n";
    //    }

    //    questObjectivesText.text = questObjectivesTextList;
    //    questRewardsText.text = quest.Reward.ToString();
    //}
}