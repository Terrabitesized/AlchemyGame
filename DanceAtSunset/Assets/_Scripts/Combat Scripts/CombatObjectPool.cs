using System.Collections.Generic;
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

    [Header("Default Pool Settings")]
    [SerializeField] private int defaultPoolSize = 10;
    [SerializeField] private bool expandPoolIfNeeded = true;

    private readonly Dictionary<GameObject, List<GameObject>> pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        CombatManager.OnCombatEnd += DisableAllObjects;
    }

    private void OnDisable()
    {
        CombatManager.OnCombatEnd -= DisableAllObjects;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void CreatePool(GameObject prefab, int amount = -1)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Cannot create a pool for a null prefab.");
            return;
        }

        if (pools.ContainsKey(prefab))
            return;

        if (amount < 0)
            amount = defaultPoolSize;

        List<GameObject> pool = new();

        pools.Add(prefab, pool);

        for (int i = 0; i < amount; i++)
        {
            CreateObject(prefab, pool);
        }
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (prefab == null)
            return null;

        if (!pools.ContainsKey(prefab))
            CreatePool(prefab);

        List<GameObject> pool = pools[prefab];

        foreach (GameObject obj in pool)
        {
            if (obj != null && !obj.activeInHierarchy)
                return obj;
        }

        if (!expandPoolIfNeeded)
            return null;

        return CreateObject(prefab, pool);
    }

    private GameObject CreateObject(GameObject prefab, List<GameObject> pool)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Add(obj);

        return obj;
    }

    public void DisableAllObjectsInAPool(GameObject prefab)
    {
        if (prefab == null)
            return;

        if (!pools.ContainsKey(prefab))
            return;

        List<GameObject> pool = pools[prefab];

        foreach (GameObject obj in pool)
            obj.SetActive(false);
    }

    public void DisableAllObjects(bool val)
    {
        foreach (var pool in pools.Values)
        {
            foreach (GameObject obj in pool)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}
