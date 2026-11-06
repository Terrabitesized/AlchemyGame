using System;
using TMPro;
using UnityEngine;

public class OverworldPlayerStats : MonoBehaviour, ISavable
{
    public static OverworldPlayerStats Instance;

    public SavablePlayerData PlayerData = new SavablePlayerData();
    public BaseStats BaseStats;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        // Make a runtime copy so the original ScriptableObject is never modified
        BaseStats = Instantiate(BaseStats);
    }

    private void Start()
    {

        // Checks if we are loading from the main menu, or another scene during a play session
        if (StaticOverworldData.LoadFromMainMenu)
        {
            // New Game
            if (StaticOverworldData.CreateNewGame)
            {
                Reset();
                // Save to selected slot
                SaveManager.Instance.SaveGame(StaticOverworldData.CurrentSaveSlot);
            }
            StaticOverworldData.LoadFromMainMenu = false;
            StaticOverworldData.CreateNewGame = false;
        }
        else
        {
            if (StaticCombatData.BaseStats != null)
                BaseStats = Instantiate(StaticCombatData.BaseStats);

            // Checks if we have won a battle right before this scene loaded
            if (StaticCombatData.experienceEarned != 0)
            {
                addExp(StaticCombatData.experienceEarned);
                StaticCombatData.experienceEarned = 0;
            }
        }
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
        // RESET STATS
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
        // ADD 100 XP
        if (Input.GetKeyDown(KeyCode.L))
        {
            addExp(100);
        }
    }

    public string DisplayPlayTime()
    {
        int hours = Mathf.FloorToInt(PlaySessionData.totalPlayTime / 3600f);
        int minutes = Mathf.FloorToInt((PlaySessionData.totalPlayTime % 3600) / 60f);
        int seconds = Mathf.FloorToInt(PlaySessionData.totalPlayTime % 60f);

        return "Time: " + hours + ":" + minutes + ":" + seconds;
    }

    public void setExp(int newExp)
    {
        PlayerData.exp = newExp;
    }
    public void addExp(int newExp)
    {
        PlayerData.exp += newExp;
        expUpHandler();
       
    }
    public int getExp()
    {
        return PlayerData.exp;
    }

    public void setMaxExp(int newMaxExp)
    {
        PlayerData.maxExp = newMaxExp;
    }
    public int getMaxExp()
    {
        return PlayerData.maxExp;
    }

    // MANAGING THE LEVEL UP
    public void levelUp()
    {
        BaseStats.level++;
        BaseStats.maxHealth += 10;
        BaseStats.currentHealth = BaseStats.maxHealth;

        PlayerData.maxExp += PlayerData.maxExp * 4 / 3;

        Debug.Log("Level: " + BaseStats.level);
        Debug.Log("XP: " + PlayerData.exp);
        Debug.Log("Health: " + BaseStats.maxHealth);

        expUpHandler();
    }


    //public Vector3 setPlayerPosition(Vector3 newPosition)
    //{
    //    PlayerData.PlayerPosition = new Vector3Serializable(newPosition);
    //    return PlayerData.PlayerPosition.ToVector3();
    //}

    //public Vector3 getPlayerPosition()
    //{
    //    return PlayerData.PlayerPosition.ToVector3();
    //}

    private void expUpHandler()
    {
        if (PlayerData.exp >= PlayerData.maxExp)
        {
            levelUp();
        }
    }

    public float getTimePlayed()
    {
        return PlaySessionData.totalPlayTime;
    }
    public void addTimePlayed(float time)
    {
        PlaySessionData.totalPlayTime += time;
    }



    public void Reset()
    {
        BaseStats.maxHealth = 100;
        BaseStats.currentHealth = 100;
        BaseStats.speed = 20;
        BaseStats.attack = 1;
        BaseStats.defense = 1;
        BaseStats.level = 1;

        PlayerData.exp = 0;
        PlayerData.maxExp = 100;
        PlayerData.totalPlayTime = 0;

        PlaySessionData.totalPlayTime = 0;
    }

    // ISavable interface implementation
    public void CaptureSaveData(GameSaveData GameSaveData)
    {
        Debug.Log($"SAVING PLAYER POSITION: {transform.position}");
        // Save player stats
        GameSaveData.PlayerStats.maxHealth = BaseStats.maxHealth;
        GameSaveData.PlayerStats.currentHealth = BaseStats.currentHealth;
        GameSaveData.PlayerStats.attack = BaseStats.attack;
        GameSaveData.PlayerStats.defense = BaseStats.defense;
        GameSaveData.PlayerStats.speed = BaseStats.speed;
        GameSaveData.PlayerStats.level = BaseStats.level;

        // Save progression
        GameSaveData.PlayerStats.exp = PlayerData.exp;
        GameSaveData.PlayerStats.maxExp = PlayerData.maxExp;

        // Save playtime
        GameSaveData.PlayerStats.totalPlayTime = PlaySessionData.totalPlayTime;

        // Save location
        GameSaveData.SceneStats.sceneName =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        GameSaveData.SceneStats.PlayerPosition =
            new Vector3Serializable(transform.position);
    }

    public void RestoreSaveData(GameSaveData GameSaveData)
    {

        if (GameSaveData == null || GameSaveData.PlayerStats == null)
        {
            Debug.LogWarning("Cannot restore OverworldStats: save PlayerData is missing.");
            return;
        }

        // Restore player stats
        BaseStats.maxHealth = GameSaveData.PlayerStats.maxHealth;
        BaseStats.currentHealth = GameSaveData.PlayerStats.currentHealth;
        BaseStats.attack = GameSaveData.PlayerStats.attack;
        BaseStats.defense = GameSaveData.PlayerStats.defense;
        BaseStats.speed = GameSaveData.PlayerStats.speed;
        BaseStats.level = GameSaveData.PlayerStats.level;

        // Restore progression
        PlayerData.exp = GameSaveData.PlayerStats.exp;
        PlayerData.maxExp = GameSaveData.PlayerStats.maxExp;

        // Restore playtime
        PlaySessionData.totalPlayTime = GameSaveData.PlayerStats.totalPlayTime;

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

    // USED PlayerData
    [Serializable]
    public class SavablePlayerData
    {
        public int exp = 0;
        public int maxExp = 100;
        public float totalPlayTime = 0f;

        // Save Location
        public string sceneName;
        public Vector3Serializable PlayerPosition;
    }
}

[Serializable]
public class PlayerStats
{
    public BaseStats PlayerBaseStats;
}
