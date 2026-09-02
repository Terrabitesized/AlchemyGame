using UnityEngine;

public class SavePoint : MonoBehaviour, Interactable
{
    public SaveMenu saveMenu;
    public KeyCode InteractionKey => KeyCode.F;
    public string InteractionText => "Press F to Save";

    public void Interact()
    {
        saveMenu.Open();
    }
}