using System;
using System.Collections;
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

    public static event Action OnMenuOpened;
    public static event Action OnMenuClosed;
    public static event Action OnMenuTabChanged;

    [SerializeField] private CanvasGroup controlsPanel;
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [SerializeField] private Dictionary<MenuState, MenuPanel> menuDatabase = new Dictionary<MenuState, MenuPanel>();
    private MenuState currentMenuState = MenuState.Null;

    private Coroutine hidingCoroutine;
    private Coroutine showingCoroutine;

    private void OnEnable()
    {
        InputHandler.PlayerInput.Overworld.OpenMenu.performed += OpenMenu;
    }

    private void OnDisable()
    {
        InputHandler.PlayerInput.Overworld.OpenMenu.performed -= OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed -= CloseMenu;
        InputHandler.PlayerInput.UI.Tab.performed -= SwitchTab;
    }

    private void Start()
    {
        menuCanvasGroup.alpha = 0f;

        foreach (var panel in menuDatabase.Keys)
        {
            menuDatabase[panel].Hide();
        }
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
        SetMenuPage(1, false);
        controlsPanel.alpha = 0f;
        menuCanvasGroup.alpha = 1f;

        OnMenuOpened?.Invoke();
    }

    private void CloseMenu(InputAction.CallbackContext context)
    {
        CloseMenu();
    }

    private void CloseMenu()
    {
        controlsPanel.alpha = 1f;
        menuCanvasGroup.alpha = 0f;
        currentMenuState = MenuState.Null;

        foreach (var panel in menuDatabase.Keys)
        {
            menuDatabase[panel].Hide();
        }

        OverworldEnemySpawning.Instance.SetAllEnemyCanMove(true);

        InputHandler.EnableOverworldInput();
        InputHandler.PlayerInput.Overworld.OpenMenu.performed += OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed -= CloseMenu;
        InputHandler.PlayerInput.UI.Tab.performed -= SwitchTab;

        OnMenuClosed?.Invoke();
    }
    #endregion

    public void SetMenuPage(int menuState, bool fireEvent = true)
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

        if(fireEvent)
            OnMenuTabChanged?.Invoke();
    }

    private void SwitchTab(InputAction.CallbackContext context)
    {
        SetMenuPage((int) currentMenuState + Mathf.RoundToInt(context.ReadValue<float>()));
    }
}
