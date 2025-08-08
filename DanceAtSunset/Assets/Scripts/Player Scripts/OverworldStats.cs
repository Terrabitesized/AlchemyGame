using System.Dynamic;
using TMPro;
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


    private void Start()
    {
        if (data == null)
        SaveToJson();

        LoadFromJson();

        // Stats display 
        expText.text = "" + getExp() + "/" + getMaxExp();

    }

    public void SaveToJson()
    {
        string playerStats = JsonUtility.ToJson(data);
        JsonUtility.FromJsonOverwrite(playerStats, data);
        string filePath = Application.persistentDataPath + "/playerStats.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, playerStats);
        Debug.Log("Save successful");
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/playerStats.json";
        string playerStats = System.IO.File.ReadAllText(filePath);
        data = JsonUtility.FromJson<myData>(playerStats);
        Debug.Log("Load Successful");
    }

    void Update()
    {
        // SAVE
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveToJson();
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

    // DISPLAY ALL STATS

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

    public void addExp(int newExp)
    {
        data.exp += newExp;
        expUpHandler();
       
    }
    public int getExp()
    {
        return data.exp;
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
        data.exp = data.exp - (int)data.maxExp;
        data.maxExp = data.maxExp * 4/3;
        Debug.Log("Level: " + getLevel());
        Debug.Log("XP: " + getExp());
        Debug.Log("Health: " + getMaxHp());
        SaveToJson();
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
        expText.text = "" + getExp() + "/" + getMaxExp();
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
        SaveToJson();
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
    }

    // END OF STAT MANAGING

    // START STAT MENU VIEWING 




}
