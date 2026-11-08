using System;
using UnityEngine;

public class OverworldPlayerStats : MonoBehaviour, ISavable
{
    public static OverworldPlayerStats Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    void Update()
    {

        // Track playtime
        PlaySessionData.totalPlayTime += Time.deltaTime;

        // DEBUGGING
        // DEBUG: SAVE CURRENT SLOT
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveManager.Instance.SaveGame(
                StaticOverworldData.CurrentSaveSlot
            );
        }

        // DEBUG: LOAD CURRENT SLOT
        if (Input.GetKeyDown(KeyCode.V))
        {
            SaveManager.Instance.LoadGame(
                StaticOverworldData.CurrentSaveSlot
            );
        }
    }

    public string DisplayPlayTime()
    {
        int hours = Mathf.FloorToInt(PlaySessionData.totalPlayTime / 3600f);
        int minutes = Mathf.FloorToInt((PlaySessionData.totalPlayTime % 3600) / 60f);
        int seconds = Mathf.FloorToInt(PlaySessionData.totalPlayTime % 60f);

        return "Time: " + hours + ":" + minutes + ":" + seconds;
    }

    public void CaptureSaveData(GameSaveData save)
    {
        PlayerData player = PlayerSession.PlayerData;

        save.PlayerStats.maxHealth = player.PlayerBaseStats.MaxHealth;
        save.PlayerStats.currentHealth = player.PlayerBaseStats.CurrentHealth;
        save.PlayerStats.attack = player.PlayerBaseStats.Attack;
        save.PlayerStats.defense = player.PlayerBaseStats.Defense;
        save.PlayerStats.level = player.CurrentLevel;

        save.PlayerStats.exp = player.CurrentExperience;

        save.PlayerStats.Gold = player.Gold;

        save.PlayerStats.totalPlayTime = player.TotalPlayTime;

        save.SceneStats.sceneName =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        save.SceneStats.PlayerPosition =
            new Vector3Serializable(transform.position);
    }

    public void RestoreSaveData(GameSaveData GameSaveData)
    {
        if (GameSaveData == null || GameSaveData.PlayerStats == null)
            return;

        PlayerSession.LoadGame(GameSaveData);


        OverworldMovement movement = GetComponent<OverworldMovement>();
        CharacterController controller = GetComponent<CharacterController>();

        // Temporarily stop player movement
        if (movement != null)
            movement.enabled = false;

        if (controller != null)
            controller.enabled = false;

        // Restore saved position
        transform.position = GameSaveData.SceneStats.PlayerPosition.ToVector3();

        Debug.Log($"PLAYER POSITION AFTER RESTORE: {transform.position}");

        // Re-enable controller
        if (controller != null)
            controller.enabled = true;

        if (movement != null)
            movement.enabled = true;
    }
}
