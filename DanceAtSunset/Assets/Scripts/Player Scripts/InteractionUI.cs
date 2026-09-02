using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    [SerializeField] private TMP_Text promptText;

    [SerializeField] private PlayerInteractionManager playerInteraction;

    private void OnEnable()
    {
        playerInteraction.OnInteractableChanged += ShowPrompt;
        playerInteraction.OnInteractableCleared += HidePrompt;
    }

    private void OnDisable()
    {
        playerInteraction.OnInteractableChanged -= ShowPrompt;
        playerInteraction.OnInteractableCleared -= HidePrompt;
    }

    private void ShowPrompt(IInteractable interactable)
    {
        prompt.SetActive(true);

        promptText.text = interactable.InteractionPrompt;
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }

    private void HidePrompt()
    {
        prompt.SetActive(false);
    }
}