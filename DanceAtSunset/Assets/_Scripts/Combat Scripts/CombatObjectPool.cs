using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CombatObjectPool : MonoBehaviour
{
    public static CombatObjectPool Instance;

    [Header("Base Ingredient Prefabs")]
    [SerializeField] private GameObject baseIngredientObject;
    private List<GameObject> baseIngredientPool = new List<GameObject>();
    [SerializeField] private int ingredientAmountToInstantiate = 10;

    [Header("Crystal Projectiles")]
    [SerializeField] private GameObject crystalProjectile;
    private List<GameObject> crystalProjectilePool = new List<GameObject>();
    [SerializeField] private int crystalAmountToInstantiate = 9;

    [Header("Damage Popup Prefabs")]
    [SerializeField] private GameObject damagePopupPrefab;
    private List<GameObject> damagePopupPool = new List<GameObject>();
    [SerializeField] private int damagePopupAmountToInstantiate = 10;

    //[Header("Ability Popup Prefabs")]
    //[SerializeField] private GameObject abilityPopupPrefab;
    //private List<GameObject> abilityPopupPool = new List<GameObject>();
    //[SerializeField] private int abilityPopupAmountToInstantiate = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        CombatManager.OnCombatEnd += DisableAllIngredients;
    }

    private void OnDisable()
    {
        CombatManager.OnCombatEnd -= DisableAllIngredients;
    }

    private void Start()
    {
        for (int i = 0; i < ingredientAmountToInstantiate; i++)
        {
            GameObject temp = Instantiate(baseIngredientObject);
            temp.SetActive(false);
            baseIngredientPool.Add(temp);
        }

        for (int i = 0; i < crystalAmountToInstantiate; i++)
        {
            GameObject temp = Instantiate(crystalProjectile);
            temp.SetActive(false);
            crystalProjectilePool.Add(temp);
        }

        for (int i = 0; i < damagePopupAmountToInstantiate; i++)
        {
            GameObject temp = Instantiate(damagePopupPrefab);
            temp.SetActive(false);
            damagePopupPool.Add(temp);
        }
    }

    public GameObject GetPooledIngredient()
    {
        for(int i = 0; i < baseIngredientPool.Count; i++)
        {
            if(!baseIngredientPool[i].activeInHierarchy)
            {
                return baseIngredientPool[i];
            }
        }

        GameObject temp = Instantiate(baseIngredientObject);
        temp.SetActive(false);
        baseIngredientPool.Add(temp);
        return temp;
    }

    public GameObject GetPooledCrystalProjectile()
    {
        for (int i = 0; i < crystalProjectilePool.Count; i++)
        {
            if (!crystalProjectilePool[i].activeInHierarchy)
            {
                return crystalProjectilePool[i];
            }
        }

        GameObject temp = Instantiate(crystalProjectile);
        temp.SetActive(false);
        crystalProjectilePool.Add(temp);
        return temp;
    }

    public GameObject GetPooledDamagePopup()
    {
        for (int i = 0; i < damagePopupPool.Count; i++)
        {
            if (!damagePopupPool[i].activeInHierarchy)
            {
                return damagePopupPool[i];
            }
        }

        GameObject temp = Instantiate(damagePopupPrefab);
        temp.SetActive(false);
        damagePopupPool.Add(temp);
        return temp;
    }

    public void DisableAllIngredients(bool val)
    {
        for (int i = 0; i < baseIngredientPool.Count; i++)
            baseIngredientPool[i].SetActive(false);
    }
}
