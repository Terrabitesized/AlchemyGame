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
public class DialogueOption
{
    public string Text;

    [SerializeReference]
    public IDialogueAction Action;
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
public interface IDialogueAction
{
    void Execute(DialogueManager manager);
}

[Serializable]
public class LoadSceneAction : IDialogueAction
{
    public string SceneName;

    public void Execute(DialogueManager manager)
    {
        manager.LoadScene(SceneName);
    }
}