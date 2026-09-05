using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
    public event Action<IInteractable> OnInteractableChanged;
    public event Action<IInteractable> OnInteractableCleared;

    private IInteractable currentInteractable;

    [SerializeField] private InputHandler inputHandler;

    private void Start()
    {
        inputHandler.PlayerInput.Overworld.Interact.performed += Interact;
    }

    private void OnDisable()
    {
        inputHandler.PlayerInput.Overworld.Interact.performed -= Interact;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
            currentInteractable.Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            SetInteractable(interactable);
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null && interactable == currentInteractable)
            ClearInteractable(interactable);
    }

    private void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
        OnInteractableChanged?.Invoke(interactable);

        interactable.InteractRangeEnter();
    }

    private void ClearInteractable(IInteractable interactable)
    {
        currentInteractable = null;
        OnInteractableCleared?.Invoke(interactable);

        interactable.InteractRangeExit();
    }
}
