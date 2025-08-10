using System.Dynamic;
using System.IO;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class OverworldStats : MonoBehaviour
{
    public myData data = new myData();

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
        // If there is no data, as with a new game, save default data
        if (data == null)
        {
            SaveToJson(1);
        }

        // Checks if we are loading from the main menu, or another scene during a play session
        if (StaticOverworldData.loadFromMainMenu)
        {
            LoadFromJson(1);
            StaticOverworldData.loadFromMainMenu = false;
        }
        else
        {
            // This implies we have finished a combat, or are moving to a new scene on the map
            setMaxHp(StaticCombatData.maxHealth);
            setAtk(StaticCombatData.playerAttack);
            setDef(StaticCombatData.playerDefense);
            setLevel(StaticCombatData.playerLevel);
            setExp(StaticCombatData.currentExp);

            // Checks if we have won a battle right before this scene loaded
            if (StaticCombatData.experienceEarned != 0)
            {
                addExp(StaticCombatData.experienceEarned);
                StaticCombatData.experienceEarned = 0;
            }

            // Sets our stats to StaticCombatData, so that if we switch scenes in overworld data is transfered
            // Our "soft save" if you will
            StaticCombatData.maxHealth = getMaxHp();
            StaticCombatData.playerAttack = getAtk();
            StaticCombatData.playerDefense = getDef();
            StaticCombatData.playerLevel = getLevel();
            StaticCombatData.currentExp = getExp();
        }


        // Stats display 

        updateStats();
    }

    public void SaveToJson(int slot)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);
        Debug.Log("Game saved to slot: " + slot);
    }

    public void LoadFromJson(int slot)
    {
        string path = GetSavePath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<myData>(json);
            updateStats();
            Debug.Log("Loaded game from slot: " + slot);
        }
        else
        {
            Debug.Log("No save file found in slot: " + slot);
            data = new myData();
        }
    }

    string GetSavePath(int slot)
    {
        return Application.persistentDataPath + "/saveSlot" + slot + ".json";
    }

    void Update()
    {

        // Track playtime
        data.totalPlayTime += Time.deltaTime;

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

    public void updateStats()
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
        int hours = Mathf.FloorToInt(data.totalPlayTime / 3600f);
        int minutes = Mathf.FloorToInt((data.totalPlayTime % 3600) / 60f);
        int seconds = Mathf.FloorToInt(data.totalPlayTime % 60f);
        return "Time: " + hours + ":" + minutes + ":" + seconds;
    }

    // GETTERS / SETTERS

    public void setHp(int newHp)
    {
        data.hp = newHp;
    }
    public int getHp()
    {
        return data.hp;
    }
    public void setMaxHp(int newMaxHp)
    {
        data.maxhp = newMaxHp;
    }
    public int getMaxHp()
    {
        return data.maxhp;
    }
    public void setSpeed(int newSpeed)
    {
        data.speed = newSpeed;
    }
    public int getSpeed()
    {
        return data.speed;
    }
    public void setAtk(int newAtk)
    {
        data.atk = newAtk;
    }
    public int getAtk()
    {
        return data.atk;
    }
    public void setDef(int newDef)
    {
        data.def = newDef;
    }
    public int getDef()
    {
        return data.def;
    }
    
    public void setExp(int newExp)
    {
        data.exp = newExp;
    }
    public void addExp(int newExp)
    {
        data.exp += newExp;
        expUpHandler();
       
    }
    public int getExp()
    {
        return data.exp;
    }

    public void setMaxExp(int newMaxExp)
    {
        data.maxExp = newMaxExp;
    }
    public int getMaxExp()
    {
        return data.maxExp;
    }

    // MANAGING THE LEVEL UP
    public void levelUp()
    {
        data.level++;
        setMaxHp(data.maxhp + 10);
        data.hp = getMaxHp();
        data.maxExp = getMaxExp() + (getMaxExp() * 4/3);
        Debug.Log("Level: " + getLevel());
        Debug.Log("XP: " + getExp());
        Debug.Log("Health: " + getMaxHp());
        expUpHandler();
    }

    public void setLevel(int newLevel)
    {
        data.level = newLevel;
    }
    public int getLevel()
    {
        return data.level;
    }

    private void expUpHandler()
    {
        if (data.exp >= data.maxExp)
        {
            levelUp();
        }
        updateStats();
    }

    public float getTimePlayed()
    {
        return data.totalPlayTime;
    }
    public void addTimePlayed(float time)
    {
        data.totalPlayTime = getTimePlayed() + time;
    }



    public void Reset()
    {
        data.hp = 100;
        data.maxhp = 100;
        data.speed = 20;
        data.atk = 1;
        data.level = 1;
        data.exp = 0;
        data.maxExp = 100;
        data.totalPlayTime = 0;
        SaveToJson(1);
    }

    // USED DATA
    [System.Serializable]
    public class myData
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
    }

    // END OF STAT MANAGING

    // START STAT MENU VIEWING 




}
