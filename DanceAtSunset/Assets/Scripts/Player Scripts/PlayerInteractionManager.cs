using System;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    public event Action<Interactable> OnInteractableChanged;
    public event Action OnInteractableCleared;

    private Interactable currentInteractable;

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            SetInteractable(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null && interactable == currentInteractable)
        {
            ClearInteractable();
        }
    }

    private void Update()
    {
        if (currentInteractable != null &&
            Input.GetKeyDown(currentInteractable.InteractionKey))
        {
            currentInteractable.Interact();
        }
    }

    private void SetInteractable(Interactable interactable)
    {
        currentInteractable = interactable;
        OnInteractableChanged?.Invoke(interactable);
    }

    private void ClearInteractable()
    {
        currentInteractable = null;
        OnInteractableCleared?.Invoke();
    }
}
