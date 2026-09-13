using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Interact to talk to them";
    [SerializeField] private DialogueData dialogueData;

    public void Interact()
    {
        DialogueManager.Instance?.SetDialogue(dialogueData.dialogue);
    }

    public void InteractRangeEnter()
    {

    }

    public void InteractRangeExit()
    {

    }
}
