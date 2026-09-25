using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestListCanvas : MonoBehaviour
{
    public static QuestListCanvas Instance;

    [Header("Quest List Fields")]
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
        QuestManager.OnQuestStarted += AddQuestUI;
        QuestManager.OnQuestCompleted += RemoveQuestUI;
    }

    private void OnDisable()
    {
        Instance = null;
        QuestManager.OnQuestStarted -= AddQuestUI;
        QuestManager.OnQuestCompleted -= RemoveQuestUI;
    }

    private void AddQuestUI(QuestInstance quest)
    {
        Debug.Log("I JUST STARTED A NEW QUEST!");
    }

    private void RemoveQuestUI(QuestInstance quest)
    {
        Debug.Log("I JUST ENDED A NEW QUEST!");
    }
}
