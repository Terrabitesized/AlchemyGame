using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueCanvas : MonoBehaviour
{
    public static DialogueCanvas Instance;

    private EventSystem eventSystem;
    [SerializeField] private List<Button> dialogueButtons;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        eventSystem = EventSystem.current;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ToggleDialogueUI(bool val)
    {
        if (canvasGroup == null)
            return;

        if(val)
        {
            canvasGroup.alpha = 1f;
            eventSystem.SetSelectedGameObject(dialogueButtons[0].gameObject);
        }
        else
            canvasGroup.alpha = 0f;
    }

    private void OnDisable()
    {
        foreach(Button button in dialogueButtons)
            button.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public List<Button> GetDialogueButtons() { return dialogueButtons; }
}
