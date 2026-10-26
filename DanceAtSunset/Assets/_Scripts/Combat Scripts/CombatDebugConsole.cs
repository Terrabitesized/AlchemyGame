using UnityEngine;

public class CombatDebugConsole : MonoBehaviour
{
    public InputHandler InputHandler;

    private CanvasGroup debugConsole;
    private bool debugConsoleEnabled;

    private void Awake()
    {
        debugConsole = GetComponent<CanvasGroup>();
        debugConsole.alpha = 0f;
        debugConsole.blocksRaycasts = false;
        debugConsole.interactable = false;

        debugConsoleEnabled = false;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void ToggleDebugConsole()
    {
        debugConsoleEnabled = !debugConsoleEnabled;

        debugConsole.alpha = debugConsoleEnabled ? 1f : 0f;
        debugConsole.interactable = debugConsoleEnabled;
    }
}
