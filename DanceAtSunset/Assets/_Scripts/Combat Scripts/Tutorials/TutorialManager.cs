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

    private InputHandler inputHandler;
    private Tutorial tutorialType;

    private TutorialBase currentTutorial;
    private Coroutine tutorialCoroutine;

    public static void SetupTutorial(InputHandler input)
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        GameObject go = new GameObject("TutorialManager");
        Instance = go.AddComponent<TutorialManager>();
        DontDestroyOnLoad(go);

        Instance.inputHandler = input;
        Instance.tutorialType = StaticCombatData.TutorialType;
    }

    private void OnEnable()
    {
        TutorialBase.OnTutorialStart += TutorialStart;
        TutorialBase.OnTutorialEnd += TutorialEnd;
    }

    private void OnDestroy()
    {
        TutorialBase.OnTutorialStart -= TutorialStart;
        TutorialBase.OnTutorialEnd -= TutorialEnd;
    }

    private void Start()
    {
        StartTutorial();
    }

    private void StartTutorial()
    {
        currentTutorial = CreateTutorial(tutorialType);

        if (currentTutorial == null)
        {
            Debug.LogWarning("No tutorial found!");
            return;
        }

        currentTutorial.Initialize(this, inputHandler);

        tutorialCoroutine = StartCoroutine(currentTutorial.RunTutorial());
    }

    private void TutorialStart()
    {
        inputHandler?.EnableUIInput();

        CombatManager.Instance.CombatFlow = false;

        CombatMovement.Instance.TogglePlayerCameraRotatable(false);

        CombatCanvas.Instance.ToggleCombatUIVisibility(false);
    }

    public void TutorialEnd()
    {
        inputHandler?.EnableCombatInput();

        CombatManager.Instance.CombatFlow = true;

        CombatMovement.Instance.TogglePlayerCameraRotatable(true);

        CombatCanvas.Instance.ToggleCombatUIVisibility(true);
    }

    private TutorialBase CreateTutorial(Tutorial type)
    {
        return type switch
        {
            Tutorial.Crystal => new CrystalTutorial(),
            Tutorial.Spells => null,
            Tutorial.Ultimate => null,
            _ => null
        };
    }
}