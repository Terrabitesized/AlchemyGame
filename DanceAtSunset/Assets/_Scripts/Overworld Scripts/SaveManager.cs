using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    // Events
    public event Action<int> OnGameSaved;
    public event Action<int> OnGameLoaded;

    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // save path for slot
    string SavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"SaveSlot{slot}.json");
    }

    // save info, may or may not use everything
    public void SaveGame(int slot)
    {
        GameSaveData data = new GameSaveData();

        // Find every component implementing ISavable
        MonoBehaviour[] components =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (MonoBehaviour component in components)
        {
            if (component is ISavable savable)
            {
                savable.CaptureSaveData(data);
            }
        }

        // Write the completed save
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(SavePath(slot), json);

        // Notify listeners
        OnGameSaved?.Invoke(slot);

        Debug.Log($"Saved Slot {slot}");
    }

    // try to load data
    public GameSaveData LoadData(int slot)
    {
        string path = SavePath(slot);

        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);

        OnGameLoaded?.Invoke(slot);

        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public void DeleteSave(int slot)
    {
        string path = SavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save found in slot {slot}");
            return;
        }

        File.Delete(path);

        Debug.Log($"Deleted Slot {slot}");
    }

    // Load into scene and position
    public void LoadGame(int slot)
    {
        Debug.Log("LOAD GAME CALLED");
        Debug.Log("SCENE LOADED, STARTING RESTORATION");
        GameSaveData data = LoadData(slot);

        if (data == null)
        {
            Debug.LogWarning($"No valid save found in slot {slot}");
            return;
        }

        Time.timeScale = 1f;

        StartCoroutine(LoadGameCoroutine(data));
    }

    private IEnumerator LoadGameCoroutine(GameSaveData data)
    {
        Debug.Log($"Loading save from slot with version {data.saveVersion}");
        AsyncOperation operation =
                    SceneManager.LoadSceneAsync(data.overworld.sceneName);

        while (!operation.isDone)
            yield return null;

        Debug.Log("SEARCHING FOR SAVABLE COMPONENTS");

        // Give the new scene its first frame to initialize.
        yield return null;

        MonoBehaviour[] components =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (MonoBehaviour component in components)
        {
            if (component is ISavable savable)
            {
                Debug.Log($"FOUND SAVABLE: {component.name}");
                savable.RestoreSaveData(data);
            }
        }

        Debug.Log("Save successfully loaded.");
    }
}