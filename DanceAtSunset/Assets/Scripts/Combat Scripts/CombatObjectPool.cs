using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CombatObjectPool : MonoBehaviour
{
    public static CombatObjectPool Instance;

    [SerializeField] private GameObject baseIngredientObject;
    private List<GameObject> baseIngredientPool = new List<GameObject>();
    [SerializeField] private int ingredientAmountToInstantiate = 10;
    private bool hasPooledIngredients = false;

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
}
