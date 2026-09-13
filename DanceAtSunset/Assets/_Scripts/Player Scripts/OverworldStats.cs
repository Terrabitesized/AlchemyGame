using System;
using System.IO;
using TMPro;
using UnityEngine;

public class OverworldStats : MonoBehaviour
{
    public PlayerData Data = new PlayerData();
    public BaseStats BaseStats = new BaseStats();

    // Displaying stats 
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI levelText;

    public Canvas statsDisplay;

    private void Awake()
    {
      statsDisplay.enabled = false;
    }


    private void Start()
    {
        // If there is no Data, as with a new game, save default Data
        if (Data == null)
        {
            SaveToJson(1);
        }

        // Checks if we are loading from the main menu, or another scene during a play session
        if (StaticOverworldData.loadFromMainMenu)
        {
            if (!StaticOverworldData.createNewGame) {
                LoadFromJson(StaticOverworldData.currentSaveSlot);
                
            } 
            else
            {
                Reset();
                SaveToJson(1);
            }
            StaticOverworldData.loadFromMainMenu = false;
        }
        else
        {
            // This implies we have finished a combat, or are moving to a new scene on the map
            //setMaxHp(StaticCombatData.maxHealth);
            //setAtk(StaticCombatData.playerAttack);
            //setDef(StaticCombatData.playerDefense);
            //setLevel(StaticCombatData.playerLevel);
            //setExp(StaticCombatData.currentExp);

            // Checks if we have won a battle right before this scene loaded
            if (StaticCombatData.experienceEarned != 0)
            {
                addExp(StaticCombatData.experienceEarned);
                StaticCombatData.experienceEarned = 0;
            }

            // Sets our stats to StaticCombatData, so that if we switch scenes in overworld Data is transfered
            // Our "soft save" if you will
            //StaticCombatData.maxHealth = getMaxHp();
            //StaticCombatData.health = getHp();
            //StaticCombatData.playerAttack = getAtk();
            //StaticCombatData.playerDefense = getDef();
            //StaticCombatData.playerLevel = getLevel();
            //StaticCombatData.currentExp = getExp();
        }


        // Stats display 

        UpdateStats();
    }

    public void SaveToJson(int slot)
    {
        Data.totalPlayTime = PlaySessionData.totalPlayTime;

        // Save current location
        Data.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        setPlayerPosition(transform.position);

        Debug.Log($"Saving: Level={Data.level}, XP={Data.exp}, Time={Data.totalPlayTime}");
        Debug.Log($"Saving location: {Data.sceneName} at {transform.position}");


        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(GetSavePath(slot), json);

        Debug.Log("Saved Slot " + slot);
    }

    public void LoadFromJson(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.Log("No save in slot " + slot);
            return;
        }

        string json = File.ReadAllText(path);
        Data = JsonUtility.FromJson<PlayerData>(json);

        // Restore session play time
        PlaySessionData.totalPlayTime = Data.totalPlayTime;
        Debug.Log($"Loaded: Level={Data.level}, XP={Data.exp}, Time={Data.totalPlayTime}");

        // If the save contains a player position, move it
        if (!Data.playerPosition.Equals(default(Vector3Serializable)) && StaticOverworldData.loadFromMainMenu)
        {
                Vector3 savedPos = Data.playerPosition.ToVector3();
                transform.position = savedPos;
        }

        UpdateStats();

        Debug.Log("Loaded Slot " + slot);
    }

    public string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"SaveSlot{slot}.json");
    }



    void Update()
    {

        // Track playtime
        PlaySessionData.totalPlayTime += Time.deltaTime;

        // DEBUGGING
        // SAVE
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveToJson(1);
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

        // LOAD STATS
        if (Input.GetKeyDown(KeyCode.V))
        {
            LoadFromJson(1);
        }
        // SHOW / HIDE STATS
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (statsDisplay.enabled == true)
            {
                statsDisplay.enabled = false;
            } else
            {
                statsDisplay.enabled = true;
            }
           

        }

    }

    // DISPLAY ALL STATS

    public void UpdateStats()
    {
        speedText.text = "Spd: " + getSpeed();
        atkText.text = "Atk: " + getAtk();
        defText.text = "Def: " + getDef();
        hpText.text = "HP: " + getMaxHp();
        expText.text = "XP: " + getExp() + "/" + getMaxExp();
        levelText.text = "Level: " + getLevel();
    }

    public string displayPlayTime()
    {
        int hours = Mathf.FloorToInt(Data.totalPlayTime / 3600f);
        int minutes = Mathf.FloorToInt((Data.totalPlayTime % 3600) / 60f);
        int seconds = Mathf.FloorToInt(Data.totalPlayTime % 60f);
        return "Time: " + hours + ":" + minutes + ":" + seconds;
    }

    // GETTERS / SETTERS

    public void setHp(int newHp)
    {
        Data.hp = newHp;
    }
    public int getHp()
    {
        return Data.hp;
    }
    public void setMaxHp(int newMaxHp)
    {
        Data.maxhp = newMaxHp;
    }
    public int getMaxHp()
    {
        return Data.maxhp;
    }
    public void setSpeed(int newSpeed)
    {
        Data.speed = newSpeed;
    }
    public int getSpeed()
    {
        return Data.speed;
    }
    public void setAtk(int newAtk)
    {
        Data.atk = newAtk;
    }
    public int getAtk()
    {
        return Data.atk;
    }
    public void setDef(int newDef)
    {
        Data.def = newDef;
    }
    public int getDef()
    {
        return Data.def;
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
        Data.level++;
        setMaxHp(Data.maxhp + 10);
        Data.hp = getMaxHp();
        Data.maxExp = getMaxExp() + (getMaxExp() * 4/3);
        Debug.Log("Level: " + getLevel());
        Debug.Log("XP: " + getExp());
        Debug.Log("Health: " + getMaxHp());
        expUpHandler();
    }

    public void setLevel(int newLevel)
    {
        Data.level = newLevel;
    }
    public int getLevel()
    {
        return Data.level;
    }

    public Vector3 setPlayerPosition(Vector3 newPosition)
    {
        Data.playerPosition = new Vector3Serializable(newPosition);
        return Data.playerPosition.ToVector3();
    }

    public Vector3 getPlayerPosition()
    {
        return Data.playerPosition.ToVector3();
    }

    private void expUpHandler()
    {
        if (Data.exp >= Data.maxExp)
        {
            levelUp();
        }

        UpdateStats();
    }

    public float getTimePlayed()
    {
        return Data.totalPlayTime;
    }
    public void addTimePlayed(float time)
    {
        Data.totalPlayTime = getTimePlayed() + time;
    }



    public void Reset()
    {
        Data.hp = 100;
        Data.maxhp = 100;
        Data.speed = 20;
        Data.atk = 1;
        Data.level = 1;
        Data.exp = 0;
        Data.maxExp = 100;
        Data.totalPlayTime = 0;
        PlaySessionData.totalPlayTime = 0;
        SaveToJson(1);
    }

    // USED DATA
    [Serializable]
    public class PlayerData
    {
        public int maxhp = 100;
        public int hp = 100;
        public int speed = 20;
        public int atk = 1;
        public int def = 1;
        public int level = 1;
        public int exp = 0;
        public int maxExp = 100;
        public float totalPlayTime = 0f;

        // Save Location
        public string sceneName;
        public Vector3Serializable playerPosition;
    }

    // END OF STAT MANAGING

    // START STAT MENU VIEWING 




}
