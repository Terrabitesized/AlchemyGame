using System;
using System.Collections;
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

    [Header("Object Slider Fields")]
    [SerializeField] private AnimationCurve objectiveSliderAnimCurve;
    [SerializeField] private float objectiveSliderAnimDuration;
    private Dictionary<QuestObjective, Coroutine> objectiveCoroutines = new Dictionary<QuestObjective, Coroutine>();

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

            objectiveCoroutines.TryAdd(objective, null);
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

    private void UpdateQuestUI(QuestInstance quest, QuestObjective objective)
    {
        Debug.Log($"The {quest.Definition.name} quest has been updated!");

        QuestUIList.TryGetValue(quest, out GameObject questUI);

        if (questUI != null)
        {
            int objectiveIndex = quest.Objectives.IndexOf(objective);

            if (objectiveIndex < 0)
                return;

            // Set text
            if (quest.Objectives[objectiveIndex].CurrentProgress >= quest.Objectives[objectiveIndex].RequiredProgress)
                questUI.transform.GetChild(objectiveIndex + 2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "COMPLETE!";
            else
            {
                questUI.transform.GetChild(objectiveIndex + 2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                $"{quest.Objectives[objectiveIndex].CurrentProgress} / {quest.Objectives[objectiveIndex].RequiredProgress}";
            }

            // Check if bar is currently animating. If so, stop it and start again
            if (objectiveCoroutines[objective] != null)
                StopCoroutine(objectiveCoroutines[objective]);

            Slider objectiveSlider = questUI.transform.GetChild(objectiveIndex + 2).transform.GetChild(2).GetComponent<Slider>();

            objectiveCoroutines[objective] = StartCoroutine(AnimateSlider(objectiveSlider, objectiveSlider.value,
                (float)quest.Objectives[objectiveIndex].CurrentProgress / quest.Objectives[objectiveIndex].RequiredProgress));
        }
    }

    private IEnumerator AnimateSlider(Slider slider, float fromPercent, float toPercent)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / objectiveSliderAnimDuration;

            slider.value = Mathf.Lerp(fromPercent, toPercent, objectiveSliderAnimCurve.Evaluate(progress));
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
