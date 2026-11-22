using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Game Database")]
public class GameDatabase : ScriptableObject
{
    #region INSTANCE_ACCESSING
    private static GameDatabase _instance;

    public static GameDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameDatabase>("GameDatabase");

                if (_instance == null)
                    Debug.LogError("GameDatabase asset could not be found!");
            }

            return _instance;
        }
    }
    #endregion

    #region GAME_DATA
    public List<Spell> Spells;
    //public List<ArmorData> Armor;
    //public List<WeaponData> Weapons;

    public Spell GetSpell(int id)
    {
        return Spells.Find(x => x.spellId == id);
    }

    //public ArmorData GetArmor(string id)
    //{
    //    return Armor.Find(x => x.ID == id);
    //}

    //public WeaponData GetWeapon(string id)
    //{
    //    return Weapons.Find(x => x.ID == id);
    //}
    #endregion
}