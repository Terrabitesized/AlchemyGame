using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    public SaveMenu saveMenu;
    public KeyCode InteractionKey => KeyCode.F;
    public string InteractionPrompt => "Press F to Save";

    public void Interact()
    {
        saveMenu.Open();
    }
}