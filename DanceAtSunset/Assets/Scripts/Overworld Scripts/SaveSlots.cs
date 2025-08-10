using UnityEngine;

public class SaveSlots : MonoBehaviour
{

    private SavePoint[] saveSlots;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SavePoint>();
    }

    public void ActivateMenu()
    {

    }

}
