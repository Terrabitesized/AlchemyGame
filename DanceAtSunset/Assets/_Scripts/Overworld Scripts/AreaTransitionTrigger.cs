using UnityEngine;

public class AreaTransitionTrigger : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private string destinationScene;
    [SerializeField] private string destinationSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        AreaTransitionManager.Instance.TransitionToArea(
            destinationScene,
            destinationSpawnPoint
        );
    }
}