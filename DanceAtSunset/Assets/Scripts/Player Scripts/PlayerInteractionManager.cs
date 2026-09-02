using System;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    public event Action<IInteractable> OnInteractableChanged;
    public event Action OnInteractableCleared;

    private IInteractable currentInteractable;

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            SetInteractable(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

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

    private void SetInteractable(IInteractable interactable)
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
