using System;
using UnityEngine;

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
    public static InputContext Context { get; private set; }

    public PlayerInputSystem PlayerInput => playerInput;
    private PlayerInputSystem playerInput;

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInputSystem();

            EnableOverworldInput();
        }
    }

    private void OnDisable()
    {
        playerInput.Overworld.Disable();
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
}
