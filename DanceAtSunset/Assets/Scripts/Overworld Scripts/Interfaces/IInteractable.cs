using UnityEngine;

public interface IInteractable
{
    public KeyCode InteractionKey { get; }
    public string InteractionPrompt { get; }

    public void Interact();
}