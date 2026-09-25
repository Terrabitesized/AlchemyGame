using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestListCanvas : MonoBehaviour
{
    public static QuestListCanvas Instance;
    public Dictionary<QuestInstance, GameObject> QuestUIList = new Dictionary<QuestInstance, GameObject>();

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
        QuestObjective.OnQuestObjectiveUpdated += UpdateQuestUI;

        LoadAllQuests();
    }

    private void OnDisable()
    {
        Instance = null;
        QuestManager.OnQuestStarted -= AddQuestUI;
        QuestManager.OnQuestCompleted -= RemoveQuestUI;
        QuestObjective.OnQuestObjectiveUpdated -= UpdateQuestUI;
    }

    // DEBUG REMOVE LATER
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            QuestManager.Instance?.HandleEvent(new EnemyKilledEvent(EnemyType.Slime));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            QuestManager.Instance?.HandleEvent(new EnemyKilledEvent(EnemyType.DesertGolem));
        }
    }
    // DEBUG REMOVE LATER

    private void AddQuestUI(QuestInstance quest)
    {
        Debug.Log("I JUST STARTED A NEW QUEST!");

        GameObject tempQuestObject = Instantiate(questUI);
        foreach(QuestObjective objective in quest.Objectives)
        {
            GameObject obj = Instantiate(questObjectiveUI);
            SetupObjectiveDisplay(obj, objective);
            obj.transform.SetParent(tempQuestObject.transform, false);
        }

        tempQuestObject.transform.SetParent(questListParent.transform, false);

        // Add to dictionary for stored reference
        QuestUIList.TryAdd(quest, tempQuestObject);
    }

    private void SetupObjectiveDisplay(GameObject objectiveObject, QuestObjective objectiveDetails)
    {
        objectiveObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objectiveDetails.ToString();
        objectiveObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{objectiveDetails.CurrentProgress} / {objectiveDetails.RequiredProgress}";
        objectiveObject.transform.GetChild(2).GetComponent<Slider>().value = (float)objectiveDetails.CurrentProgress / objectiveDetails.RequiredProgress;
        Debug.Log(objectiveDetails.ToString());
        Debug.Log($"RequiredProgress is {objectiveDetails.RequiredProgress}");
    }

    private void RemoveQuestUI(QuestInstance quest)
    {
        Debug.Log("I JUST ENDED A NEW QUEST!");

        QuestUIList.TryGetValue(quest, out GameObject questUI);

        if (questUI != null)
        {
            QuestUIList.Remove(quest);
            Destroy(questUI);
        }
    }

    private void UpdateQuestUI(QuestInstance quest)
    {
        Debug.Log($"The {quest.Definition.name} quest has been updated!");

        QuestUIList.TryGetValue(quest, out GameObject questUI);

        if (questUI != null)
        {
            for (int i = 0; i < quest.Objectives.Count; i++)
            {
                questUI.transform.GetChild(i + 2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    $"{quest.Objectives[i].CurrentProgress} / {quest.Objectives[i].RequiredProgress}";
                questUI.transform.GetChild(i + 2).transform.GetChild(2).GetComponent<Slider>().value =
                    (float)quest.Objectives[i].CurrentProgress / quest.Objectives[i].RequiredProgress;
            }
        }
    }

    private void LoadAllQuests()
    {
        foreach(QuestInstance quest in QuestManager.Instance?.GetActiveQuests())
        {
            AddQuestUI(quest);
        }
    }
}
