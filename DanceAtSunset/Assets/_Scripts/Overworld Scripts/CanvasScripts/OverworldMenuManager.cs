using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldMenuManager : MonoBehaviour
{
    public InputHandler InputHandler;
    private CanvasGroup menuCanvasGroup;

    private void OnEnable()
    {
        menuCanvasGroup = GetComponent<CanvasGroup>();
        InputHandler.PlayerInput.Overworld.OpenMenu.performed += OpenMenu;
    }

    private void OnDisable()
    {
        InputHandler.PlayerInput.Overworld.OpenMenu.performed -= OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed -= CloseMenu;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CloseMenu();
    }

    private void OpenMenu(InputAction.CallbackContext context)
    {
        InputHandler.EnableUIInput();
        InputHandler.PlayerInput.Overworld.OpenMenu.performed -= OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed += CloseMenu;

        OverworldEnemySpawning.Instance.SetAllEnemyCanMove(false);

        // Show menu
        menuCanvasGroup.alpha = 1f;
    }

    private void CloseMenu(InputAction.CallbackContext context)
    {
        CloseMenu();
    }

    private void CloseMenu()
    {
        menuCanvasGroup.alpha = 0f;

        OverworldEnemySpawning.Instance.SetAllEnemyCanMove(true);

        InputHandler.EnableOverworldInput();
        InputHandler.PlayerInput.Overworld.OpenMenu.performed += OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed -= CloseMenu;
    }
}
