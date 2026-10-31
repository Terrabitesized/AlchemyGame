using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialBase
{
    public static Action OnTutorialStart;
    public static Action OnTutorialEnd;

    protected TutorialManager tutorialManager;
    protected InputHandler inputHandler;

    protected List<TutorialStep> steps = new();

    public virtual void Initialize(
        TutorialManager tutorialManager,
        InputHandler inputHandler)
    {
        this.tutorialManager = tutorialManager;
        this.inputHandler = inputHandler;

        SetupSteps();
    }

    protected abstract void SetupSteps();

    public IEnumerator RunTutorial()
    {
        OnTutorialStart?.Invoke();

        foreach (TutorialStep step in steps)
        {
            step.Begin();

            yield return new WaitUntil(() => step.IsComplete);

            step.Cleanup();
        }

        OnTutorialEnd?.Invoke();
        OnTutorialComplete();
    }

    protected virtual void OnTutorialComplete()
    {
        Debug.Log("Tutorial Complete!");
    }
}

public class CrystalTutorial : TutorialBase
{
    protected override void SetupSteps()
    {
        steps.Add(new ActionStep(() =>
        {
            Debug.Log("Welcome to the tutorial!");
            CombatManager.Instance.CombatFlow = true;
            CombatCanvas.Instance.SetPopup("Welcome to the tutorial! Click to continue", 999f);
            CombatCanvas.Instance.ToggleCombatUIVisibility(false);
        }));

        steps.Add(new WaitForContinueStep(inputHandler));

        steps.Add(new ActionStep(() =>
        {
            CombatItemSpawnManager.Instance.SpawnItemAtPosition(0, new Vector3(-10f, 0f, 0f));
            CombatCanvas.Instance.SetPopup("Go collect that crystal", 999f);
        }));

        steps.Add(new WaitForContinueStep(inputHandler));

        steps.Add(new ActionStep(() =>
        {
            CombatCanvas.Instance.HideCurrentPopup();

            inputHandler?.EnableCombatInput();
            CombatMovement.Instance.TogglePlayerCameraRotatable(true);
            CombatManager.Instance.EnemiesCanAttack = false;
            Debug.Log("Now collect a crystal!");
        }));

        // Add a step that waits for crystal collection here.
        steps.Add(new WaitForCrystalStep());

        steps.Add(new ActionStep(() =>
        {
            inputHandler?.EnableUIInput();
            CombatCanvas.Instance.SetPopup("Good! Get 3 of those and rock this hoe's shit.", 999f);
        }));

        steps.Add(new WaitForContinueStep(inputHandler));

        steps.Add(new ActionStep(() =>
        {
            CombatCanvas.Instance.HideCurrentPopup();
        }));
    }
}
