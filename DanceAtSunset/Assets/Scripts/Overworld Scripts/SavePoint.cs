using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public SaveMenu saveMenu;

    bool playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.F))
        {
            saveMenu.Open();
        }
    }
}