using System;
using TMPro;
using UnityEngine;

public class OverworldPlayerStats : MonoBehaviour, ISavable
{
    public static OverworldPlayerStats Instance;

    public PlayerData Data = new PlayerData();
    public BaseStats stats;

    // Displaying stats 
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI levelText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        // Make a runtime copy so the original ScriptableObject is never modified
        stats = Instantiate(stats);
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
                stats = Instantiate(StaticCombatData.BaseStats);

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

    // GETTERS / SETTERS

    public void setHp(int newHp)
    {
        stats.currentHealth = newHp;
    }

    public int getHp()
    {
        return stats.currentHealth;
    }
    public void setMaxHp(int newMaxHp)
    {
        stats.maxHealth = newMaxHp;
    }

    public int getMaxHp()
    {
        return stats.maxHealth;
    }

    public void setSpeed(int newSpeed)
    {
        stats.speed = newSpeed;
    }

    public int getSpeed()
    {
        return stats.speed;
    }

    public void setAtk(int newAtk)
    {
        stats.attack = newAtk;
    }

    public int getAtk()
    {
        return stats.attack;
    }

    public void setDef(int newDef)
    {
        stats.defense = newDef;
    }

    public int getDef()
    {
        return stats.defense;
    }

    public void setLevel(int newLevel)
    {
        stats.level = newLevel;
    }

    public int getLevel()
    {
        return stats.level;
    }

    public void setExp(int newExp)
    {
        Data.exp = newExp;
    }
    public void addExp(int newExp)
    {
        Data.exp += newExp;
        expUpHandler();
       
    }
    public int getExp()
    {
        return Data.exp;
    }

    public void setMaxExp(int newMaxExp)
    {
        Data.maxExp = newMaxExp;
    }
    public int getMaxExp()
    {
        return Data.maxExp;
    }

    // MANAGING THE LEVEL UP
    public void levelUp()
    {
        stats.level++;
        stats.maxHealth += 10;
        stats.currentHealth = stats.maxHealth;

        Data.maxExp += Data.maxExp * 4 / 3;

        Debug.Log("Level: " + stats.level);
        Debug.Log("XP: " + Data.exp);
        Debug.Log("Health: " + stats.maxHealth);

        expUpHandler();
    }


    //public Vector3 setPlayerPosition(Vector3 newPosition)
    //{
    //    Data.PlayerPosition = new Vector3Serializable(newPosition);
    //    return Data.PlayerPosition.ToVector3();
    //}

    //public Vector3 getPlayerPosition()
    //{
    //    return Data.PlayerPosition.ToVector3();
    //}

    private void expUpHandler()
    {
        if (Data.exp >= Data.maxExp)
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
        stats.maxHealth = 100;
        stats.currentHealth = 100;
        stats.speed = 20;
        stats.attack = 1;
        stats.defense = 1;
        stats.level = 1;

        Data.exp = 0;
        Data.maxExp = 100;
        Data.totalPlayTime = 0;

        PlaySessionData.totalPlayTime = 0;
    }

    // ISavable interface implementation
    public void CaptureSaveData(GameSaveData data)
    {
        Debug.Log($"SAVING PLAYER POSITION: {transform.position}");
        // Save player stats
        data.PlayerStats.maxHealth = stats.maxHealth;
        data.PlayerStats.currentHealth = stats.currentHealth;
        data.PlayerStats.attack = stats.attack;
        data.PlayerStats.defense = stats.defense;
        data.PlayerStats.speed = stats.speed;
        data.PlayerStats.level = stats.level;

        // Save progression
        data.PlayerStats.exp = Data.exp;
        data.PlayerStats.maxExp = Data.maxExp;

        // Save playtime
        data.PlayerStats.totalPlayTime = PlaySessionData.totalPlayTime;

        // Save location
        data.PlayerStats.sceneName =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        data.PlayerStats.PlayerPosition =
            new Vector3Serializable(transform.position);
    }

    public void RestoreSaveData(GameSaveData data)
    {

        if (data == null || data.PlayerStats == null)
        {
            Debug.LogWarning("Cannot restore OverworldStats: save data is missing.");
            return;
        }

        // Restore player stats
        stats.maxHealth = data.PlayerStats.maxHealth;
        stats.currentHealth = data.PlayerStats.currentHealth;
        stats.attack = data.PlayerStats.attack;
        stats.defense = data.PlayerStats.defense;
        stats.speed = data.PlayerStats.speed;
        stats.level = data.PlayerStats.level;

        // Restore progression
        Data.exp = data.PlayerStats.exp;
        Data.maxExp = data.PlayerStats.maxExp;

        // Restore playtime
        PlaySessionData.totalPlayTime = data.PlayerStats.totalPlayTime;

        OverworldMovement movement = GetComponent<OverworldMovement>();
        CharacterController controller = GetComponent<CharacterController>();

        // Temporarily stop player movement
        if (movement != null)
            movement.enabled = false;

        if (controller != null)
            controller.enabled = false;

        // Restore saved position
        transform.position = data.PlayerStats.PlayerPosition.ToVector3();

        Debug.Log($"PLAYER POSITION AFTER RESTORE: {transform.position}");

        // Re-enable controller
        if (controller != null)
            controller.enabled = true;

        if (movement != null)
            movement.enabled = true;
    }

    // USED DATA
    [Serializable]
    public class PlayerData
    {
        public SavedStats stats = new SavedStats();
        public int exp = 0;
        public int maxExp = 100;
        public float totalPlayTime = 0f;

        // Save Location
        public string sceneName;
        public Vector3Serializable PlayerPosition;
    }

    [Serializable]
    public class SavedStats
    {
        public int maxHealth;
        public int currentHealth;
        public int attack;
        public int defense;
        public int speed;
        public int level;
    }

    // END OF STAT MANAGING

    // START STAT MENU VIEWING 




}
