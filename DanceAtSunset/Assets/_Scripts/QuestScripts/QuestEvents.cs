public abstract class QuestEvent
{
}

public class EnemyKilledEvent : QuestEvent
{
    public EnemyType EnemyType;

    public EnemyKilledEvent(EnemyType enemyType)
    {
        EnemyType = enemyType;
    }
}

public class PlayerEnteredLocationEvent : QuestEvent
{
    public string LocationID;

    public PlayerEnteredLocationEvent(string locationID)
    {
        LocationID = locationID;
    }
}