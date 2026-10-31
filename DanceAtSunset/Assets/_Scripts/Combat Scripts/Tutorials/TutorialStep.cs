using System;
using UnityEngine.InputSystem;

public abstract class TutorialStep
{
    public bool IsComplete { get; protected set; }

    public abstract void Begin();

    public virtual void Cleanup() { }
}

public class WaitForContinueStep : TutorialStep
{
    private InputHandler inputHandler;

    public WaitForContinueStep(InputHandler inputHandler)
    {
        this.inputHandler = inputHandler;
    }

    public override void Begin()
    {
        IsComplete = false;

        inputHandler.PlayerInput.UI.Continue.performed += OnContinue;
    }

    private void OnContinue(InputAction.CallbackContext context)
    {
        IsComplete = true;
    }

    public override void Cleanup()
    {
        inputHandler.PlayerInput.UI.Continue.performed -= OnContinue;
    }
}

public class WaitForCrystalStep : TutorialStep
{
    private InputHandler inputHandler;

    public WaitForCrystalStep(InputHandler inputHandler)
    {
        this.inputHandler = inputHandler;
    }

    public override void Begin()
    {
        IsComplete = false;

        DamageCrystal.OnCrystalCollected += OnContinue;
    }

    private void OnContinue(CombatCrystal c)
    {
        IsComplete = true;
    }

    public override void Cleanup()
    {
        DamageCrystal.OnCrystalCollected -= OnContinue;
    }
}

public class ActionStep : TutorialStep
{
    private Action action;

    public ActionStep(Action action)
    {
        this.action = action;
    }

    public override void Begin()
    {
        action?.Invoke();
        IsComplete = true;
    }
}