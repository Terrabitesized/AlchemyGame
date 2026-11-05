using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public enum SaveMenuState
{
    Main,
    Confirm,
    Delete,
    Null
}

public class SaveMenu : MonoBehaviour
{
    public static SaveMenu Instance;
    [SerializeField] private InputHandler inputHandler;

    public GameObject panel;


    [Header("Save Slots")]
    public GameObject DefaultSaveSlotObject;
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;

    [Header("Overwrite Confirmation")]
    public GameObject OverwritePanel;
    public GameObject DefaultOverwriteObject;
    public TextMeshProUGUI overwriteText;

    private int pendingSaveSlot;

    [Header("Delete Confirmation")]
    public GameObject DeletePanel;
    public GameObject DefaultDeleteObject;
    public TextMeshProUGUI deleteText;

    [SerializeField] private SaveMenuState saveMenuState;
    private int pendingDeleteSlot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        panel.SetActive(false);
        saveMenuState = SaveMenuState.Null;
        
        if (OverwritePanel!= null)
        OverwritePanel.SetActive(false);

        if (DeletePanel != null)
            DeletePanel.SetActive(false);
    }

    private void Update()
    {
        if (saveMenuState == SaveMenuState.Null)
            return;

        switch (saveMenuState)
        {
            case SaveMenuState.Main:
                if(EventSystem.current.currentSelectedGameObject == null)
                    EventSystem.current.SetSelectedGameObject(DefaultSaveSlotObject);
                break;
            case SaveMenuState.Confirm:
                if (EventSystem.current.currentSelectedGameObject == null)
                    EventSystem.current.SetSelectedGameObject(DefaultOverwriteObject);
                break;
            case SaveMenuState.Delete:
                if (EventSystem.current.currentSelectedGameObject == null)
                    EventSystem.current.SetSelectedGameObject(DefaultDeleteObject);
                break;
        }
    }

    public void Open()
    {
        panel.SetActive(true);
        saveMenuState = SaveMenuState.Main;

        RefreshSlots();

        inputHandler.EnableUIInput();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(DefaultSaveSlotObject);
    }

    public void Close()
    {
        panel.SetActive(false);
        saveMenuState = SaveMenuState.Null;

        inputHandler.EnableOverworldInput();
    }

   // Retrieves play info from each save and shows
    private void RefreshSlots()
    {
        slot1Text.text = GetSlotInfo(1);
        slot2Text.text = GetSlotInfo(2);
        slot3Text.text = GetSlotInfo(3);
    }

    // Gets play info
    private string GetSlotInfo(int slot)
    {
        GameSaveData data = SaveManager.Instance.LoadData(slot);

        if (data == null)
            return $"Slot {slot}\nEmpty";

        if (data.Overworld == null)
            return $"Slot {slot}\nInvalid Save";

        return
            $"Slot {slot}\n" +
            $"Level {data.Overworld.level}\n" +
            $"Time {FormatTime(data.Overworld.totalPlayTime)}";
    }

    // time format lol
    private string FormatTime(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    // Button hooks
    public void SaveSlot1() => ConfirmSave(1);
    public void SaveSlot2() => ConfirmSave(2);
    public void SaveSlot3() => ConfirmSave(3);



    // call stats and save there
    private void ConfirmSave(int slot)
    {
        pendingSaveSlot = slot;
        saveMenuState = SaveMenuState.Confirm;

        overwriteText.text =
            $"Are you sure you want to overwrite Save Slot {slot}?";

        OverwritePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(DefaultOverwriteObject);
    }

    public void ConfirmOverwrite()
    {
        Save(pendingSaveSlot);

        OverwritePanel.SetActive(false);
    }

    public void CancelOverwrite()
    {
        OverwritePanel.SetActive(false);
        saveMenuState = SaveMenuState.Main;
        EventSystem.current.SetSelectedGameObject(DefaultSaveSlotObject);
    }

    // Delete button hooks
    public void DeleteSlot1() => ConfirmDelete(1);
    public void DeleteSlot2() => ConfirmDelete(2);
    public void DeleteSlot3() => ConfirmDelete(3);

    private void ConfirmDelete(int slot)
    {
        pendingDeleteSlot = slot;
        saveMenuState = SaveMenuState.Delete;

        deleteText.text =
            $"Are you sure you want to delete Save Slot {slot}?";

        DeletePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(DefaultDeleteObject);
    }

    public void ConfirmDelete()
    {
        SaveManager.Instance.DeleteSave(pendingDeleteSlot);
        saveMenuState = SaveMenuState.Main;
        EventSystem.current.SetSelectedGameObject(DefaultSaveSlotObject);

        DeletePanel.SetActive(false);

        RefreshSlots();
    }

    public void CancelDelete()
    {
        DeletePanel.SetActive(false);
        saveMenuState = SaveMenuState.Main;
    }

    private void Save(int slot)
    {
        SaveManager.Instance.SaveGame(slot);

        RefreshSlots();
        Close();
    }

    public void LoadSlot1() => Load(1);
    public void LoadSlot2() => Load(2);
    public void LoadSlot3() => Load(3);

    private void Load(int slot)
    {
        SaveManager.Instance.LoadGame(slot);
    }
}