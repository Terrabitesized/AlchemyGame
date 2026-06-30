using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // save path for slot
    string SavePath(int slot)
    {
        return Application.persistentDataPath + "/save" + slot + ".json";
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
}