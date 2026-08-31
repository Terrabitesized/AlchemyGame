using UnityEngine;
using TMPro;
using System.IO;

public class SaveMenu : MonoBehaviour
{
    public GameObject panel;
    public OverworldStats stats;

    [Header("Slot Texts")]
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void Open()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;

        RefreshSlots(); 
    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
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
        string path = Application.persistentDataPath + $"/SaveSlot{slot}.json";

        // Check if the save file exists
        if (!File.Exists(path))
            return $"Slot {slot}\nEmpty";

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<OverworldStats.myData>(json);

        return
            $"Slot {slot}\n" +
            $"Level {data.level}\n" +
            $"Time {FormatTime(data.totalPlayTime)}";
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
    public void SaveSlot1() => Save(1);
    public void SaveSlot2() => Save(2);
    public void SaveSlot3() => Save(3);

    // call stats and save there
    private void Save(int slot)
    {
        stats.SaveToJson(slot);
        RefreshSlots(); 
        Close();
    }

    public void MenuLoadSet(int slot)
    {
        StaticOverworldData.loadFromMainMenu = true;
        StaticOverworldData.currentSaveSlot = slot;
    }


}