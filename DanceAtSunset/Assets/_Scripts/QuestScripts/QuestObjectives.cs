using System;
using UnityEngine;

[Serializable]
public abstract class QuestObjective
{
    [NonSerialized]
    public int CurrentProgress;

    public int RequiredProgress = 1;

    public bool IsComplete =>
        CurrentProgress >= RequiredProgress;

    public abstract void Initialize();

    public abstract void HandleEvent(QuestEvent questEvent);

    public abstract QuestObjective CreateInstance();
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

    public override QuestObjective CreateInstance()
    {
        return new GoToLocationObjective
        {
            RequiredProgress = RequiredProgress,
            LocationID = LocationID
        };
    }
}