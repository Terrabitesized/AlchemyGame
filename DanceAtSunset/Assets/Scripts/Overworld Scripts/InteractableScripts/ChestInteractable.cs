using UnityEngine;
using UnityEngine.InputSystem;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "This is an interface that can just be used on anything interactable";

    public void Interact()
    {
        // Implement the logic to open the chest here
        Debug.Log("Chest opened!");
    }

    public void InteractRangeEnter()
    {

    }

    public void InteractRangeExit()
    {

    }
}
