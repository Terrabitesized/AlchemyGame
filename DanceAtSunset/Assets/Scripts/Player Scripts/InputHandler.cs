using UnityEngine;
using UnityEngine.InputSystem;

public enum InputContext
{
    Overworld,
    Combat,
    Dialogue,
    UI
}

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;
    public PlayerInputSystem PlayerInput { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        PlayerInput = new PlayerInputSystem();
    }

    private void OnEnable()
    {
        PlayerInput.Overworld.Enable();
    }
}
