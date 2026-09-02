using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    public KeyCode InteractionKey => KeyCode.F;
    public string InteractionPrompt => "Press F to Save";

    public void Interact()
    {
        SaveMenu.Instance?.Open();
    }
}