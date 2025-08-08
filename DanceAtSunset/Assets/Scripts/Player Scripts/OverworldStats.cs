using System.Dynamic;
using UnityEngine;

public class OverworldStats : MonoBehaviour
{
    public myData data = new myData();

    private void Start()
    {
        if (data == null)
        SaveToJson();
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

    public void LoadFromJason()
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

    // GETTERS / SETTERS

    public void setMaxHp(int newMaxHp)
    {
        data.maxhp = newMaxHp;
    }
    public int getMaxHp()
    {
        return data.maxhp;
    }
    public void setSpeed(float newSpeed)
    {
        data.speed = newSpeed;
    }
    public float getSpeed()
    {
        return data.speed;
    }
    public void setAtk(float newAtk)
    {
        data.atk = newAtk;
    }
    public float getAtk()
    {
        return data.atk;
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

    // MANAGING THE LEVEL UP
    public void levelUp()
    {
        data.level++;
        setMaxHp(data.maxhp + 10);
        data.hp = getMaxHp();
        data.exp = data.exp - (int)data.maxExp;
        data.maxExp = data.maxExp * 1.5f;
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
        public float speed = 20;
        public float atk = 1;
        public float def = 1;
        public int level = 1;
        public int exp = 0;
        public float maxExp = 100;
    }

    // END OF STAT MANAGING

    // START STAT MENU VIEWING 




}
