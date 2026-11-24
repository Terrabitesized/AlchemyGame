using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MenuState
{
    Null,
    Main,
    Spells,
    Inventory,
    Quests,
    Settings
}

public class OverworldMenuManager : MonoBehaviour
{
    public InputHandler InputHandler;

    [SerializeField] private Dictionary<MenuState, MenuPanel> menuDatabase = new Dictionary<MenuState, MenuPanel>();
    private MenuState currentMenuState = MenuState.Null;

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

    private void Start()
    {
        CloseMenu();
    }

    #region MENU_TOGGLING
    private void OpenMenu(InputAction.CallbackContext context)
    {
        InputHandler.EnableUIInput();
        InputHandler.PlayerInput.Overworld.OpenMenu.performed -= OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed += CloseMenu;
        InputHandler.PlayerInput.UI.Tab.performed += SwitchTab;

        OverworldEnemySpawning.Instance.SetAllEnemyCanMove(false);

        // Show menu
        SetMenuPage(1);
        menuCanvasGroup.alpha = 1f;
    }

    private void CloseMenu(InputAction.CallbackContext context)
    {
        CloseMenu();
    }

    private void CloseMenu()
    {
        menuCanvasGroup.alpha = 0f;

        foreach (var panel in menuDatabase.Keys)
        {
            menuDatabase[panel].Hide();
        }

        OverworldEnemySpawning.Instance.SetAllEnemyCanMove(true);

        InputHandler.EnableOverworldInput();
        InputHandler.PlayerInput.Overworld.OpenMenu.performed += OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed -= CloseMenu;
        InputHandler.PlayerInput.UI.Tab.performed -= SwitchTab;
    }
    #endregion

    public void SetMenuPage(int menuState)
    {
        if (menuState == (int) currentMenuState)
            return;

        if(menuState < 1)
            menuState = 5;

        if(menuState > 5)
            menuState = 1;

        MenuState newState = (MenuState) menuState;

        menuDatabase.TryGetValue(currentMenuState, out MenuPanel currentMenuPage);
        menuDatabase.TryGetValue(newState, out MenuPanel newMenuPage);

        currentMenuPage?.Hide();
        newMenuPage?.Show();

        currentMenuState = newState;
    }

    private void SwitchTab(InputAction.CallbackContext context)
    {
        int newTab = Mathf.RoundToInt(context.ReadValue<float>());

        SetMenuPage((int) currentMenuState + newTab);
    }
}
