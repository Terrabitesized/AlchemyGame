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

    [SerializeField] private Dictionary<MenuState, GameObject> menuDatabase = new Dictionary<MenuState, GameObject>();
    private MenuState currentMenuState = MenuState.Main;

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

    public void SetMenuPage(int menuState)
    {
        MenuState newState = (MenuState) menuState;

        Debug.Log($"Changing to MenuStat {menuState}");

        menuDatabase.TryGetValue(currentMenuState, out GameObject currentMenuPage);
        menuDatabase.TryGetValue(newState, out GameObject newMenuPage);

        if (currentMenuPage == null)
            Debug.LogWarning("CURRENTMENUPAGE NULL");

        if (newMenuPage == null)
            Debug.LogWarning("NEWMENUPAGE NULL");

        currentMenuPage?.SetActive(false);
        newMenuPage?.SetActive(true);

        currentMenuState = newState;
    }

    private void OpenMenu(InputAction.CallbackContext context)
    {
        InputHandler.EnableUIInput();
        InputHandler.PlayerInput.Overworld.OpenMenu.performed -= OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed += CloseMenu;

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
            menuDatabase[panel].SetActive(false);
        }

        OverworldEnemySpawning.Instance.SetAllEnemyCanMove(true);

        InputHandler.EnableOverworldInput();
        InputHandler.PlayerInput.Overworld.OpenMenu.performed += OpenMenu;
        InputHandler.PlayerInput.UI.Cancel.performed -= CloseMenu;
    }
}
