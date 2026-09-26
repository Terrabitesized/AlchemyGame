using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
public class DialogueQuestPrompt : IDialogueItem
{
    public Quest Quest;
    public int AcceptIndex;
    public int DeclineIndex;

    public void Read(DialogueManager manager)
    {
        QuestManager.Instance?.PromptQuest(Quest, manager, AcceptIndex, DeclineIndex);
        manager.ToggleDialogueVisiblity(false);
        manager.ToggleAdvanceInput(false);
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

[Serializable]
public class CheckForQuestStatus : IDialogueItem
{
    public Quest Quest;
    public int UnacceptedID;
    public int AcceptedID;
    public int OnCompleteID;
    public int CompletedID;

    public void Read(DialogueManager manager)
    {
        // Check if the quest has already been marked as complete
        if(QuestManager.Instance.GetCompletedQuests().Contains(Quest))
        {
            manager.ContinueDialogue(CompletedID);
            return;
        }

        // If not, determine the state (Unaccepted, accepted, completed but not yet marked as such)
        QuestInstance instance = QuestManager.Instance.GetQuestInstanceFromQuestID(Quest.QuestID);

        if(instance != null)
        {
            if(instance.IsComplete)
            {
                QuestManager.Instance?.CheckQuestCompletion(instance);
                manager.ContinueDialogue(OnCompleteID);
            }
            else
                manager.ContinueDialogue(AcceptedID);
        }
        else
            manager.ContinueDialogue(UnacceptedID);
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
public class GiveQuestAction : DialogueAction
{
    public Quest Quest;
    public override void Execute(DialogueManager manager)
    {
        QuestManager.Instance?.StartQuest(Quest);
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