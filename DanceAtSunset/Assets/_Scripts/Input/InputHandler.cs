using UnityEngine;

public enum InputContext
{
    Overworld,
    Combat,
    Dialogue,
    UI
}

[CreateAssetMenu(menuName = "InputHandler")]
public class InputHandler : ScriptableObject
{
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
    }

    public void EnableCombatInput()
    {
        playerInput.Combat.Enable();

        playerInput.Overworld.Disable();
        playerInput.UI.Disable();
    }

    public void EnableUIInput()
    {
        playerInput.UI.Enable();

        playerInput.Overworld.Disable();
        playerInput.Combat.Disable();
    }
}
