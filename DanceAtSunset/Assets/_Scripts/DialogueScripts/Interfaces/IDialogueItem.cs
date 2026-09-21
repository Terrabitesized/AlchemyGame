using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IDialogueItem
{
    public void Read(DialogueManager manager);
}

[Serializable]
public class DialogueText : IDialogueItem
{
    public string Text;

    public void Read(DialogueManager manager)
    {
        manager.DisplayText(Text);
    }
}

[Serializable]
public class DialogueEnd : IDialogueItem
{
    public static Action OnDialogueEnded;

    public void Read(DialogueManager manager)
    {
        manager.EndDialogue();
        OnDialogueEnded?.Invoke();
    }
}

[Serializable]
public class DialogueOption
{
    public string Text;

    [SerializeReference]
    private DialogueAction action;

    public DialogueAction Action => action;
}

[Serializable]
public class DialogueChoice : IDialogueItem
{
    public DialogueOption OptionA;
    public DialogueOption OptionB;

    public void Read(DialogueManager manager)
    {
        manager.ShowOptions(OptionA, OptionB);
    }
}

// DIALOGUE ACTIONS V
[Serializable]
public abstract class DialogueAction
{
    public abstract void Execute(DialogueManager manager);
}

[Serializable]
public class ContinueDialogueAction : DialogueAction
{
    public int dialogueID;

    public override void Execute(DialogueManager manager)
    {
        manager.ContinueDialogue(dialogueID);
    }
}

[Serializable]
public class EndDialogueAction : DialogueAction
{
    public static Action OnDialogueEnded;

    public override void Execute(DialogueManager manager)
    {
        manager.EndDialogue();
        OnDialogueEnded?.Invoke();
    }
}

[Serializable]
public class LoadCombatAction : DialogueAction
{
    public string SceneName;
    public List<GameObject> Enemies;
    public CombatType CombatType;

    public override void Execute(DialogueManager manager)
    {
        manager.LoadCombatScene(SceneName, Enemies, CombatType);
    }
}

[Serializable]
public class FireEventAction : DialogueAction
{
    public static Action OnFireEvent;
    public bool EndDialogueAfterEvent = false;

    public override void Execute(DialogueManager manager)
    {
        OnFireEvent?.Invoke();

        if(EndDialogueAfterEvent)
            manager.EndDialogue();
    }
}