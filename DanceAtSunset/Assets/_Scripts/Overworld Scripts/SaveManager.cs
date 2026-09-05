using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveManager : MonoBehaviour
{
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
        SaveData data = new SaveData();

        OverworldStats player = FindFirstObjectByType<OverworldStats>();

        data.playerPosition = new Vector3Serializable(player.transform.position);
        data.playerLevel = player.getLevel();
        data.sceneName = SceneManager.GetActiveScene().name;
        data.saveTime = System.DateTime.Now.ToString();

        File.WriteAllText(
            SavePath(slot),
            JsonUtility.ToJson(data, true)
        );

        Debug.Log("Saved Slot " + slot);
    }

    // try to load data
    public SaveData LoadData(int slot)
    {
        string path = SavePath(slot);

        if (!File.Exists(path))
            return null;

        return JsonUtility.FromJson<SaveData>(
            File.ReadAllText(path)
        );
    }

    // Load into scene and position
    public void LoadGame(int slot)
    {
        Debug.Log("Loading game from slot " + slot);

        Time.timeScale = 1f; 
        string path = SavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found for slot " + slot);
            return;
        }

        string json = File.ReadAllText(path);

        OverworldStats.myData data = JsonUtility.FromJson<OverworldStats.myData>(json);

        Debug.Log("LoadData returned: " + (data != null));

        if (data == null)
        {
            Debug.LogError("Failed to load save data from slot " + slot);
            return;
        }

        Debug.Log("Saved scene: " + data.sceneName);
        Debug.Log("Saved player position: " + data.playerPosition.ToVector3());

        StaticOverworldData.loadFromMainMenu = true;

        StartCoroutine(LoadGameCoroutine(data));
    }

    private IEnumerator LoadGameCoroutine(OverworldStats.myData data)
    {
        Debug.Log("Loading game from scene: " + data.sceneName);

        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(data.sceneName);

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log(
                "Loading scene: " +
                data.sceneName +
                " progress: " +
                asyncLoad.progress
            );

            yield return null;
        }

        Debug.Log("Scene finished loading into memory.");
        Debug.Log("Activating scene...");

        asyncLoad.allowSceneActivation = true;

        // Wait for activation
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}