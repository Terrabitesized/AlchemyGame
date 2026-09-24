using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestCanvas : MonoBehaviour
{
    public static QuestCanvas Instance;

    [Header("Quest UI Fields")]
    [SerializeField] private CanvasGroup questCanvasGroup;
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questObjectivesText;
    [SerializeField] private TextMeshProUGUI questRewardsText;
    [SerializeField] private Button questAcceptButton;
    [SerializeField] private Button questDeclineButton;

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

        // Setup buttons
        questAcceptButton.onClick.AddListener(() =>
        {
            QuestManager.Instance?.StartQuest(quest);
            manager.ContinueDialogue(acceptIndex);
            manager.ToggleDialogueVisiblity(true);
            manager.ToggleAdvanceInput(true);

            questCanvasGroup.alpha = 0f;

            questAcceptButton.onClick.RemoveAllListeners();
            questDeclineButton.onClick.RemoveAllListeners();
        });

        questDeclineButton.onClick.AddListener(() =>
        {
            manager.ContinueDialogue(declineIndex);
            manager.ToggleDialogueVisiblity(true);
            manager.ToggleAdvanceInput(true);

            questCanvasGroup.alpha = 0f;

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
