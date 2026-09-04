using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public string InteractionPrompt { get; }

    public void Interact();
    public void InteractRangeEnter();
    public void InteractRangeExit();
}