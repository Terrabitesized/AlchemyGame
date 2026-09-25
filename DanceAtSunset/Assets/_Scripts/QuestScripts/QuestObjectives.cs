using System;

[Serializable]
public abstract class QuestObjective
{
    // If completed, send true
    public static Action<bool> OnObjectiveUpdated;

    [NonSerialized]
    public int CurrentProgress;

    public int RequiredProgress = 1;

    public bool IsComplete =>
        CurrentProgress >= RequiredProgress;

    public abstract void Initialize();

    public abstract void HandleEvent(QuestEvent questEvent);

    public abstract QuestObjective CreateInstance();

    public abstract override string ToString();
}

public enum EnemyType
{
    Slime,
    DesertGolem,
    DesertGolemMinion
}

[Serializable]
public class KillEnemyObjective : QuestObjective
{
    public EnemyType EnemyType;

    public override void Initialize()
    {
        CurrentProgress = 0;
    }

    public override void HandleEvent(QuestEvent questEvent)
    {
        if (questEvent is EnemyKilledEvent enemyKilled)
        {
            if (enemyKilled.EnemyType == EnemyType)
            {
                CurrentProgress++;
            }
        }
    }

    public override string ToString()
    {
        string result = string.Empty;

        if (RequiredProgress > 1)
            result = $"- Defeat a {EnemyType}";
        else
            result = $"- Defeat {RequiredProgress} {EnemyType}s";

        return result;
    }

    public override QuestObjective CreateInstance()
    {
        return new KillEnemyObjective
        {
            RequiredProgress = RequiredProgress,
            EnemyType = EnemyType
        };
    }
}

[Serializable]
public class GoToLocationObjective : QuestObjective
{
    public string LocationID;

    public override void Initialize()
    {
        CurrentProgress = 0;
    }

    public override void HandleEvent(QuestEvent questEvent)
    {
        if (questEvent is PlayerEnteredLocationEvent locationEvent)
        {
            if (locationEvent.LocationID == LocationID)
            {
                CurrentProgress = RequiredProgress;
            }
        }
    }

    public override string ToString()
    {
        return $"- Head to {LocationID}";
    }

    public override QuestObjective CreateInstance()
    {
        return new GoToLocationObjective
        {
            RequiredProgress = RequiredProgress,
            LocationID = LocationID
        };
    }
}