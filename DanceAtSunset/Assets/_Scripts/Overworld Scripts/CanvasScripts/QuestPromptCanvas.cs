using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestPromptCanvas : MonoBehaviour
{
    public static QuestPromptCanvas Instance;

    [Header("Quest Proposal UI Fields")]
    [SerializeField] private CanvasGroup questCanvasGroup;
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questObjectivesText;
    [SerializeField] private TextMeshProUGUI questRewardsText;
    [SerializeField] private Button questAcceptButton;
    [SerializeField] private Button questDeclineButton;

    [Header("Quest Viewer Fields")]
    [SerializeField] private CanvasGroup questListCanvasGroup;
    [SerializeField] private Transform questListParent;
    [SerializeField] private GameObject questUI;
    [SerializeField] private GameObject questObjectiveUI;

    private void OnEnable()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDisable()
    {
        Instance = null;
    }

    public void PromptQuest(Quest quest, DialogueManager manager, int acceptIndex, int declineIndex)
    {
        // Setup screen
        PopulateQuestInfo(quest);
        questCanvasGroup.alpha = 1f;
        questCanvasGroup.blocksRaycasts = true;

        // Setup buttons
        questAcceptButton.onClick.AddListener(() =>
        {
            QuestManager.Instance?.StartQuest(quest);
            manager.ContinueDialogue(acceptIndex);
            manager.ToggleDialogueVisiblity(true);
            manager.ToggleAdvanceInput(true);

            questCanvasGroup.alpha = 0f;
            questCanvasGroup.blocksRaycasts = false;

            questAcceptButton.onClick.RemoveAllListeners();
            questDeclineButton.onClick.RemoveAllListeners();
        });

        questDeclineButton.onClick.AddListener(() =>
        {
            manager.ContinueDialogue(declineIndex);
            manager.ToggleDialogueVisiblity(true);
            manager.ToggleAdvanceInput(true);

            questCanvasGroup.alpha = 0f;
            questCanvasGroup.blocksRaycasts = false;

            questAcceptButton.onClick.RemoveAllListeners();
            questDeclineButton.onClick.RemoveAllListeners();
        });

        EventSystem.current.SetSelectedGameObject(questAcceptButton.gameObject);
    }

    private void PopulateQuestInfo(Quest quest)
    {
        questNameText.text = quest.QuestName;
        questDescriptionText.text = quest.Description;

        string questObjectivesTextList = "";
        for (int i = 0; i < quest.Objectives.Count; i++)
        {
            questObjectivesTextList += quest.Objectives[i].ToString();

            if (i < quest.Objectives.Count - 1)
                questObjectivesTextList += "\n";
        }

        questObjectivesText.text = questObjectivesTextList;
        questRewardsText.text = quest.Reward.ToString();
    }
}
