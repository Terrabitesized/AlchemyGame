using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public enum InputContext
{
    Null,
    Overworld,
    Combat,
    Dialogue,
    UI
}

[CreateAssetMenu(menuName = "InputHandler")]
public class InputHandler : ScriptableObject
{
    public static event Action<InputContext> OnInputContextChanged;
    public static event Action<bool> OnInputDeviceChanged;
    public static InputContext Context { get; private set; }
    public static bool IsUsingGamepad { get; private set; }

    public PlayerInputSystem PlayerInput => playerInput;
    private PlayerInputSystem playerInput;

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInputSystem();

            EnableOverworldInput();
        }

        InputSystem.onActionChange += OnInputActionChange;
    }

    private void OnDisable()
    {
        playerInput.Overworld.Disable();

        InputSystem.onActionChange -= OnInputActionChange;
    }

    public void EnableOverworldInput()
    {
        playerInput.Overworld.Enable();

        playerInput.Combat.Disable();
        playerInput.UI.Disable();

        Context = InputContext.Overworld;
        OnInputContextChanged?.Invoke(InputContext.Overworld);
    }

    public void EnableCombatInput()
    {
        playerInput.Combat.Enable();

        playerInput.Overworld.Disable();
        playerInput.UI.Disable();

        Context = InputContext.Combat;
        OnInputContextChanged?.Invoke(InputContext.Combat);
    }

    public void EnableUIInput()
    {
        playerInput.UI.Enable();

        playerInput.Overworld.Disable();
        playerInput.Combat.Disable();

        Context = InputContext.UI;
        OnInputContextChanged?.Invoke(InputContext.UI);
    }

    public void DisableAllInput()
    {
        playerInput.UI.Disable();
        playerInput.Overworld.Disable();
        playerInput.Combat.Disable();

        Context = InputContext.Null;
        OnInputContextChanged?.Invoke(InputContext.Null);
    }

    private void OnInputActionChange(object action, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed)
            return;

        if (action is not InputAction inputAction)
            return;

        // Ignore actions outside our input asset.
        if (inputAction.actionMap?.asset != playerInput.asset)
            return;

        InputControl control = inputAction.activeControl;

        if (control == null)
            return;

        bool gamepad = control.device is Gamepad;

        if (IsUsingGamepad == gamepad)
            return;

        IsUsingGamepad = gamepad;
        OnInputDeviceChanged?.Invoke(gamepad);

        Debug.Log(
    $"Action: {inputAction.actionMap.name}/{inputAction.name} | " +
    $"Control: {control.path} | " +
    $"Device: {control.device.name}"
);
    }

}
