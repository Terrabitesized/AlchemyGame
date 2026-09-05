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

            playerInput.Overworld.Enable();
        }
    }

    private void OnDisable()
    {
        playerInput.Overworld.Disable();
    }
}
