using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class CombatItemSpawnManager : MonoBehaviour
{
    [Header("Spawn Parameters")]
    [SerializeField] private float itemSpawnInterval = .5f;
    [SerializeField] private float ingerientDespawnTime = 5f;

    [SerializeField] private Dictionary<GameObject, float> spawnDictionary = new Dictionary<GameObject, float>();

    [Header("Damage Crystal Parameters")]
    [SerializeField] private CombatCrystal[] spawnawbleCrystals;
    [SerializeField] private List<ICombatSpawnable> spawnedObjects = new List<ICombatSpawnable>();
    [Range(0f, 1f)][SerializeField] private float ingredientSubAbilityChance;
    [SerializeField] private Ability[] ingredientSubAbilities;
    [SerializeField] private GameObject ingredientModel;
    [SerializeField] private GameObject spellCrystalModel;

    private readonly int CRYSTAL_COLOR = Shader.PropertyToID("CrystalColor");
    private readonly int LIFETIME = Shader.PropertyToID("Lifetime");

    private Coroutine itemSpawnCoroutine;

    private void OnEnable()
    {
        CombatManager.OnCombatStart += StartSpawning;
        CombatManager.OnCombatEnd += StopSpawning;

        UltimateManager.OnUltinateStart += ClearAllSpawnedCrystals;
    }

    private void OnDisable()
    {
        CombatManager.OnCombatStart -= StartSpawning;
        CombatManager.OnCombatEnd -= StopSpawning;

        UltimateManager.OnUltinateStart -= ClearAllSpawnedCrystals;
    }

    private void Start()
    {
        CombatObjectPool.Instance?.CreatePool(ingredientModel);
    }

    private void StartSpawning(int x)
    {
        itemSpawnCoroutine = StartCoroutine(SpawnItems());
    }

    private void StopSpawning(bool x)
    {
        if(itemSpawnCoroutine != null)
            StopCoroutine(itemSpawnCoroutine);
    }

    private IEnumerator SpawnItems()
    {
        float arenaSize = CombatManager.Instance.ArenaSize;

        // Spawns ingredients until combat is over
        while (true)
        {
            if(spawnDictionary.Count == 0)
            {
                Debug.LogError("Spawn Dictionary was empty! No objects will be spawned");
                yield break;
            }

            // Waits a specified duration before spawning a new ingredient
            yield return GameFlowUtility.WaitForGameplaySeconds(itemSpawnInterval);


            // Determines position within circle for ingredient
            float x_Pos = Random.Range(-arenaSize, arenaSize);
            float z_Pos = Random.Range(-arenaSize, arenaSize);

            while (Vector2.Distance(new Vector2(x_Pos, z_Pos), new Vector2(0.0f, 0.0f)) > arenaSize)
            {
                x_Pos = Random.Range(-arenaSize, arenaSize);
                z_Pos = Random.Range(-arenaSize, arenaSize);
            }

            // TODO CYRENE: ADD CHANCE FOR SPELL CRYSTAL TO SPAWN AND UNIQUE LOGIC FOR SPAWNING

            // Determine which item should be spawned
            float totalWeight = 0;
            GameObject spawnedObject = null;

            foreach(var (spawnableObject, weight) in spawnDictionary)
            {
                totalWeight += weight;
            }

            float rand = Random.Range(0f, totalWeight);

            foreach (var (spawnableObject, weight) in spawnDictionary)
            {
                if (rand < weight)
                {
                    spawnedObject = spawnableObject;
                    break;
                }
                else
                    rand -= weight;
            }

            if(spawnedObject == null)
            {
                Debug.LogError("Random selection process broke!");
                yield break;
            }

            

            // Attempts to grab an ingredient from the pool
            GameObject temp = CombatObjectPool.Instance?.GetPooledObject(spawnedObject);

            // Ensure that pool was not maxed out
            if (temp != null && temp.GetComponentInChildren<DamageCrystal>() != null)
            {
                temp.SetActive(true);

                // Spawns ingredient, assigns location, time til despawn, color, and sub ability if applicable from available pool
                DamageCrystal ingredientScript = temp.GetComponentInChildren<DamageCrystal>();

                // Add to the list of collected ingredients
                spawnedObjects.Add(ingredientScript);

                CombatCrystal c = spawnawbleCrystals[Random.Range(0, spawnawbleCrystals.Length)];

                ingredientScript.crystal =
                    spawnawbleCrystals[Random.Range(0, spawnawbleCrystals.Length)];

                temp.GetComponentInChildren<VisualEffect>().SetFloat(LIFETIME, ingerientDespawnTime);
                temp.GetComponentInChildren<VisualEffect>().SetVector4(CRYSTAL_COLOR, ingredientScript.crystal.color);

                ingredientScript.despawnTime = ingerientDespawnTime;

                temp.transform.position = new Vector3(x_Pos, 0f, z_Pos);

                float sub = Random.Range(0f, 1f);
                if (sub <= ingredientSubAbilityChance)
                {
                    int subAbilityIndex = Random.Range(0, ingredientSubAbilities.Length);

                    ingredientScript.subAbilities.Add(ingredientSubAbilities[subAbilityIndex]);
                    ingredientScript.SetSubIcon(subAbilityIndex);
                }
                else // If there is no sub ability, we need to set the icon to transparent
                    ingredientScript.SetSubIcon(-1);
            }
            else
            {
                temp.SetActive(true);
                temp.transform.position = new Vector3(x_Pos, 0f, z_Pos);
            }
        }

    }

    public void RemoveSpawnable(ICombatSpawnable spawnable)
    {
        spawnedObjects.Remove(spawnable);
    }

    public void HideAllSpawnables()
    {
        foreach (DamageCrystal c in spawnedObjects)
        {
            // TODO CYRENE: Implement. But also good luck cause it's VFX graph...
        }
    }

    public void ClearAllSpawnedCrystals()
    {
        foreach (DamageCrystal c in spawnedObjects)
        {
            StartCoroutine(c.DisableSelf(0f));
        }

        spawnedObjects.Clear();
    }

    // DEBUG TOOLS V
    private void Update()
    {
        // DEBUG INGREDIENT ADDING
        if (CombatManager.Instance.GetCollectedCrystalsCount() < 3)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                DamageCrystal.ManuallyInvokeCrystalCollected(spawnawbleCrystals[0]);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                DamageCrystal.ManuallyInvokeCrystalCollected(spawnawbleCrystals[2]);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                DamageCrystal.ManuallyInvokeCrystalCollected(spawnawbleCrystals[1]);
            }
        }
    }
}

[Serializable]
public class SpawnItem
{
    public ICombatSpawnable SpawnedItem;
    public float SpawnWeighting;
}
