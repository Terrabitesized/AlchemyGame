using UnityEngine;

public class AreaQuestTesting : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            QuestManager.Instance?.HandleEvent(new PlayerEnteredLocationEvent("Desert"));
        }
    }
}
