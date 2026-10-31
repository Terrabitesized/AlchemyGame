using UnityEngine;
using UnityEngine.InputSystem;

public enum Tutorial
{
    None,
    Crystal,
    Spells,
    Ultimate
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    private static Tutorial tutorialType;
    private static InputHandler inputHandler;

    public static void SetupTutorial(InputHandler input)
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Debug.Log("TUTORIAL INSTANCE CREATED");

        GameObject go = new GameObject("TutorialManager");
        Instance = go.AddComponent<TutorialManager>();
        DontDestroyOnLoad(go);

        tutorialType = StaticCombatData.TutorialType;
        inputHandler = input;
    }

    // TODO CYRENE: Abstract this out into a "Tutorial" abstract class with overrides
    private void Start()
    {
        inputHandler?.EnableUIInput();
        CombatManager.Instance.CombatFlow = false;
        CombatMovement.Instance.TogglePlayerCameraRotatable(false);
        CombatCanvas.Instance.ToggleCombatUIVisibility(false);

        inputHandler.PlayerInput.UI.Continue.performed += TestAdvance;
    }

    private void TestAdvance(InputAction.CallbackContext context)
    {
        Debug.Log("Hi");

        inputHandler.PlayerInput.UI.Continue.performed -= TestAdvance;
    }

    private void OnDestroy()
    {
        // This is apperently completely safe in C#. Who knew.
        inputHandler.PlayerInput.UI.Continue.performed -= TestAdvance;
    }

}
