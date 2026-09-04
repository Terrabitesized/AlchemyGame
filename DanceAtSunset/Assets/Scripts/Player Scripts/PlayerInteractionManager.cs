using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
    public event Action<IInteractable> OnInteractableChanged;
    public event Action<IInteractable> OnInteractableCleared;

    public PlayerInputSystem Controls;

    private IInteractable currentInteractable;

    private void Awake()
    {
        Controls = new PlayerInputSystem();
    }

    private void OnEnable()
    {
        Controls.Player.Enable();
    }

    private void OnDisable()
    {
        Controls.Player.Disable();
    }

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
            ClearInteractable(interactable);
        }
    }

    private void Update()
    {
        if (currentInteractable != null && Controls.Player.Interact.WasPressedThisFrame())
        {
            currentInteractable.Interact();
        }
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
