using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] private InputHandler inputHandler;

    [SerializeReference]
    private List<IDialogueItem> dialogueItems = new List<IDialogueItem>();

    private int currentDialogueIndex;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private List<Button> dialogueButtons;
    private CanvasGroup canvasGroup;

    #region Unity Functions
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        inputHandler.PlayerInput.UI.Continue.started += Continue;
    }

    private void OnDisable()
    {
        inputHandler.PlayerInput.UI.Continue.started -= Continue;

        foreach (Button button in dialogueButtons)
            button.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    #endregion

    #region Dialgoue Functions
    public void ToggleDialogueUI(bool val)
    {
        if (canvasGroup == null)
            return;

        if(val)
        {
            canvasGroup.alpha = 1f;

            // Disable movement and enable UI
            inputHandler.EnableUIInput();
        }
        else
        {
            canvasGroup.alpha = 0f;
            inputHandler.EnableOverworldInput();
        }
    }

    public void SetDialogue(List<IDialogueItem> dialogue)
    {
        dialogueItems.Clear();

        dialogueItems = new List<IDialogueItem>(dialogue);
        StartDialogue();
    }

    public void StartDialogue()
    {
        currentDialogueIndex = 0;

        ToggleDialogueUI(true);
        Read();
    }

    public void EndDialogue()
    {
        ToggleDialogueUI(false);

        currentDialogueIndex = 0;
    }

    public void Read()
    {
        Debug.Log($"THE CURRENT DIALOGUE HAS {dialogueItems.Count}");

        if (currentDialogueIndex >= dialogueItems.Count)
        {
            Debug.Log("BY DIALOGUE IS OVER???");
            EndDialogue();
            return;
        }

        Debug.Log("WE SHOULD BE READING>>>");
        dialogueItems[currentDialogueIndex].Read(this);
    }

    public void Continue(InputAction.CallbackContext context)
    {
        currentDialogueIndex++;
        Read();
    }

    public void DisplayText(string text)
    {
        dialogueText.text = text;

        // Hide dialogue options when displaying normal text
        foreach (Button button in dialogueButtons)
            button.gameObject.SetActive(false);
    }

    public void ShowOptions(DialogueOption optionA, DialogueOption optionB)
    {
        dialogueButtons[0].gameObject.SetActive(true);
        dialogueButtons[1].gameObject.SetActive(true);

        dialogueButtons[0].GetComponentInChildren<TMP_Text>().text = optionA.Text;
        dialogueButtons[1].GetComponentInChildren<TMP_Text>().text = optionB.Text;

        dialogueButtons[0].onClick.RemoveAllListeners();
        dialogueButtons[1].onClick.RemoveAllListeners();

        dialogueButtons[0].onClick.AddListener(() =>
        {
            optionA.Action?.Execute(this);
        });

        dialogueButtons[1].onClick.AddListener(() =>
        {
            optionB.Action?.Execute(this);
        });

        EventSystem.current.SetSelectedGameObject(dialogueButtons[0].gameObject);
    }

    public void LoadScene(string sceneName)
    {
        EndDialogue();
        SceneManager.LoadScene(sceneName);
    }
    #endregion

    public List<Button> GetDialogueButtons() { return dialogueButtons; }
}
