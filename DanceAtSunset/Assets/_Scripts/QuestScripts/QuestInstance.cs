using System.Collections.Generic;

public class QuestInstance
{
    public Quest Definition { get; }

    public bool QuestAccepted
    {
        get
        {
            if (QuestManager.Instance.GetActiveQuests().Contains(this))
                return true;

            return false;
        }
    }

    public List<QuestObjective> Objectives { get; }

    public bool IsComplete
    {
        get
        {
            foreach (QuestObjective objective in Objectives)
            {
                if (!objective.IsComplete)
                    return false;
            }

            return true;
        }
    }

    public QuestInstance(Quest definition)
    {
        Definition = definition;

        Objectives = new List<QuestObjective>();

        foreach (QuestObjective objective in definition.Objectives)
        {
            Objectives.Add(objective.CreateInstance());
        }
    }

    public void Initialize()
    {
        foreach (QuestObjective objective in Objectives)
        {
            objective.Initialize();
        }
    }

    public void HandleEvent(QuestEvent questEvent)
    {
        foreach (QuestObjective objective in Objectives)
        {
            if (!objective.IsComplete)
            {
                objective.HandleEvent(questEvent, this);
            }
        }
    }
}

public class QuestObjectiveInstance
{
    public QuestObjective Definition { get; }

    public int CurrentProgress { get; private set; }

    public bool IsComplete =>
        CurrentProgress >= Definition.RequiredProgress;

    public QuestObjectiveInstance(QuestObjective definition)
    {
        Definition = definition;
        CurrentProgress = 0;
    }
}