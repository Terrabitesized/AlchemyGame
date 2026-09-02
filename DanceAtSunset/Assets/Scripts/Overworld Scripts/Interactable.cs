using UnityEngine;

public interface Interactable
{
    KeyCode InteractionKey { get; }
    string InteractionText { get; }

    void Interact();
}