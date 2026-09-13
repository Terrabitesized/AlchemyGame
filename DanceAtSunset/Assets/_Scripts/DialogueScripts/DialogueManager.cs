using System.Collections;
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



    [SerializeField] private float typingSpeed = .05f;
    [SerializeReference] private List<IDialogueItem> dialogueItems = new List<IDialogueItem>();

    private int currentDialogueIndex;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private string currentText = "";
    private Coroutine typingCoroutine;
    private bool canAdvance = false;

    [Header("Dialogue Fields")]
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
        isDialogueActive = true;

        ToggleDialogueUI(true);
        Read();
    }

    public void EndDialogue()
    {
        Debug.Log($"ENDING DIALOGUE AT INDEX: {currentDialogueIndex}");

        isDialogueActive = false;
        isTyping = false;
        canAdvance = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = "";

        foreach (Button button in dialogueButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }

        ToggleDialogueUI(false);

        currentDialogueIndex = 0;

        Debug.Log($"DIALOGUE ENDED. ACTIVE = {isDialogueActive}");
    }

    public void Continue(InputAction.CallbackContext context)
    {
        if (!isDialogueActive)
            return;

        // A press while text is typing = finish the text
        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        // A press after typing = advance dialogue
        if (!canAdvance)
            return;

        currentDialogueIndex++;
        StartCoroutine(ReadNextFrame());
    }

    public void ContinueDialogue(int dialogueID)
    {
        if (!isDialogueActive)
            return;

        currentDialogueIndex = dialogueID;

        StartCoroutine(ReadNextFrame());
    }

    private IEnumerator ReadNextFrame()
    {
        yield return null;
        Read();
    }

    public void Read()
    {
        if (!isDialogueActive)
            return;

        if (currentDialogueIndex >= dialogueItems.Count)
        {
            EndDialogue();
            return;
        }

        dialogueItems[currentDialogueIndex].Read(this);
    }

    public void DisplayText(string text) { typingCoroutine = StartCoroutine(DisplayTextCoroutine(text));  }

    private IEnumerator DisplayTextCoroutine(string text)
    {
        currentText = text;
        isTyping = true;
        canAdvance = false;

        foreach (Button button in dialogueButtons)
            button.gameObject.SetActive(false);

        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        CompleteTyping();
    }

    private void CompleteTyping()
    {
        if (!isTyping)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        dialogueText.text = currentText;

        ToggleAdvanceInput(true);

        // Automatically move to a choice immediately after this text
        if (currentDialogueIndex + 1 < dialogueItems.Count &&
            dialogueItems[currentDialogueIndex + 1] is DialogueChoice)
        {
            currentDialogueIndex++;
            StartCoroutine(ReadNextFrame());
        }
    }

    public void ShowOptions(DialogueOption optionA, DialogueOption optionB)
    {
        // Disable Continue input while choosing
        ToggleAdvanceInput(false);

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

    public void ToggleAdvanceInput(bool val)
    {
        canAdvance = val;
    }

    public void LoadCombatScene(string sceneName, List<GameObject> enemies, CombatType combatType)
    {
        EndDialogue();

        StaticCombatData.SetupCombat(OverworldManager.Instance?.GetPlayer(), enemies);
        StaticCombatData.CombatType = combatType;

        if (ScreenShatter.Instance != null)
            StartCoroutine(ScreenShatter.Instance.TakeScreenshot());

        SceneManager.LoadScene(sceneName);
    }
    #endregion

    public List<Button> GetDialogueButtons() { return dialogueButtons; }
}
