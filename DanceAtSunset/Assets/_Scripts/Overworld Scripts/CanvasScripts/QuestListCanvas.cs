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
        objectiveObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"0 / {objectiveDetails.RequiredProgress}";
        objectiveObject.transform.GetChild(2).GetComponent<Slider>().value = 0f;
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

    }
}
