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

    [Header("Damage Popup Prefabs")]
    [SerializeField] private GameObject damagePopupPrefab;
    private List<GameObject> damagePopupPool = new List<GameObject>();
    [SerializeField] private int damagePopupAmountToInstantiate = 10;

    [Header("Ability Popup Prefabs")]
    [SerializeField] private GameObject abilityPopupPrefab;
    private List<GameObject> abilityPopupPool = new List<GameObject>();
    [SerializeField] private int abilityPopupAmountToInstantiate = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < ingredientAmountToInstantiate; i++)
        {
            GameObject temp = Instantiate(baseIngredientObject);
            temp.SetActive(false);
            baseIngredientPool.Add(temp);
        }

        for (int i = 0; i < damagePopupAmountToInstantiate; i++)
        {
            GameObject temp = Instantiate(damagePopupPrefab);
            temp.SetActive(false);
            damagePopupPool.Add(temp);
        }

        for (int i = 0; i < abilityPopupAmountToInstantiate; i++)
        {
            GameObject temp = Instantiate(abilityPopupPrefab);
            temp.SetActive(false);
            abilityPopupPool.Add(temp);
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

        return null;
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

        return null;
    }

    public GameObject GetPooledAbilityPopup()
    {
        for (int i = 0; i < abilityPopupPool.Count; i++)
        {
            if (!abilityPopupPool[i].activeInHierarchy)
            {
                return abilityPopupPool[i];
            }
        }

        return null;
    }
}
