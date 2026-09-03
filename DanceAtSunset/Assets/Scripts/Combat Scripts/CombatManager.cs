using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using static MusicManager;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public bool isBattleOver = false;
    [SerializeField] private PotionManager pm;
    [SerializeField] private GameObject victoryCam;
    [SerializeField] private GameObject canvas;
    private bool finalSequencePlaying = false;

    [Header("Inherited Variables")]
    [SerializeField] private float ingerientSpawnInterval = .5f;
    [SerializeField] private float ingerientDespawnTime = 5f;
    [SerializeField] private CombatIngredient[] spawnawbleIngredients;
    [SerializeField] private GameObject ingredientModel;
    [SerializeField] private TextMeshProUGUI subtitles;

    [SerializeField] private GameObject player;

    [Header("Dynamic Combat Variables")]
    [SerializeField] private List<CombatIngredient> collectedIngredients;
    [SerializeField] private List<GameObject> enemiesInCombat;
    private int numOfIngredients = 0;

    [Header("Victory Variables")]
    [SerializeField] private int experienceEarned = 0;
    [SerializeField] private int damageDealt = 0;
    [SerializeField] private int damageTaken = 0;
    [SerializeField] private int ingredientsCollected = 0;
    [SerializeField] private int timeTaken = 0;
    private bool wantsToCast;

    // Combat actions
    public static event Action<int> OnCombatStart; // # of enemies present
    public static event Action<bool> OnCombatEnd; // true if win, false if lose

    public static event Action OnIngredientsManuallyCleared; // Fires when the player manually clears their ingredients

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        // Spawn the player's prefav, will need loaded stats at a later point
        GameObject playPrefab = Instantiate(player);
        playPrefab.transform.position = new Vector3(0f, 1.12f, -10f);

        player = playPrefab;

        // Load data from StaticCombatData
        enemiesInCombat = StaticCombatData.enemies;

        // Lock player mouse
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        PotionManager.OnSpellCast += ClearIngredients;
    }

    private void OnDisable()
    {
        PotionManager.OnSpellCast -= ClearIngredients;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        // Start counting battle duration
        StartCoroutine(increaseTimeTaken());

        // Check to make sure enemy data was properly loaded
        if (enemiesInCombat == null || enemiesInCombat.Count == 0)
        {
            isBattleOver = true;
        } else
        {
            // Spawn enemies based on what enemies were in spawned roamable in overworld
            if (enemiesInCombat.Count > 0)
            {
                // Spawn the only enemy in center
                if (enemiesInCombat.Count == 1)
                {
                    GameObject temp = Instantiate(enemiesInCombat[0]);
                    temp.transform.position = new Vector3(0f, 1f, 0f);

                    // Reassign reference to clone, as to not modify prefab
                    enemiesInCombat[0] = temp;
                }
                else
                {
                    // Spawn enemies slightly spread apart
                    for (int i = 0; i < enemiesInCombat.Count; i++)
                    {
                        GameObject temp = Instantiate(enemiesInCombat[i]);
                        float x_Pos = UnityEngine.Random.Range(-5f, 5f);
                        float z_Pos = UnityEngine.Random.Range(-5f, 5f);
                        temp.transform.position = new Vector3(x_Pos, 1f, z_Pos);

                        // Reassign reference to clone, as to not modify prefab
                        enemiesInCombat[i] = temp;
                    }
                }
            }
            else
            {
                // End combat
                isBattleOver = true;
            }

            // Set up Potion Manager
            pm.SetupPM(player, enemiesInCombat);

            // Begin spawning Inregedients
            StartCoroutine("SpawnIngredients");

            // Invoke combat start event
            OnCombatStart?.Invoke(enemiesInCombat.Count);
            
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(!isBattleOver)
        {
            // DEBUG INGREDIENT ADDING
            if(numOfIngredients < 3)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    AddIngredient(spawnawbleIngredients[0]);
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    AddIngredient(spawnawbleIngredients[1]);
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    AddIngredient(spawnawbleIngredients[2]);
                }
            }
            // DEBUG INGREDIENT ADDING

            // Allows player to dispense their collected ingredients
            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("Cleared potions");
                OnIngredientsManuallyCleared?.Invoke();
                ClearIngredients();
            }

            // Allows player to dispense their collected ingredients
            if (Input.GetKey(KeyCode.Space))
            {
                Time.timeScale = .25f;
            } else
            {
                Time.timeScale = 1f;
            }

            if(Input.GetKeyDown(KeyCode.F) && wantsToCast)
            {
                // Do potion thing based on ingredients
                Debug.Log("Girl hello????");
                CastCurrentSpell();


                // Call function to brew potion
                // Clear collectedIngredients
                // Set numOfIngredients to 0.
                //ClearIngredients();
            }

            // Checks if game should end
            if (player.GetComponent<PlayerStats>().getHP() <= 0)
            {
                Debug.Log("Player has died! Game should end");
                isBattleOver = true;

                if (!finalSequencePlaying)
                {
                    OnCombatEnd?.Invoke(false);

                    canvas.GetComponent<CombatCanvas>().DefeatCanvas(player);

                    finalSequencePlaying = true;
                }
            }
            if (enemiesInCombat.Count == 0)
            {
                Debug.Log("All enemies have died! Game should end");
                isBattleOver = true;

                StaticCombatData.experienceEarned = experienceEarned;

                if(!finalSequencePlaying)
                {
                    OnCombatEnd?.Invoke(true);

                    canvas.GetComponent<CombatCanvas>().VictoryCanvas(player, victoryCam, 
                        experienceEarned, damageDealt, damageTaken, ingredientsCollected,
                        timeTaken);

                    finalSequencePlaying = true;
                }
            }
        }
    }

    public void AddIngredient(CombatIngredient ing)
    {
        collectedIngredients.Add(ing);

        // Up victory tally
        ingredientsCollected++;

        if(numOfIngredients == 0)
        {
            subtitles.SetText(ing.ingredientName);
        } else if (numOfIngredients > 0)
        {
            subtitles.SetText(subtitles.text + " + " + ing.ingredientName);
        }

        numOfIngredients++;

        if (numOfIngredients == 3)
        {
            wantsToCast = true;
            pm.PrimeSpell(CalculateIngredients());
        }
        
    }

    private void CastCurrentSpell()
    {
        pm.CastCurrentSpell();
    }

    private void ClearIngredients()
    {
        collectedIngredients.Clear();
        
        pm.ResetCurrentSpell();

        numOfIngredients = 0;
        subtitles.SetText("Empty");
    }

    private void ClearIngredients(Spell spell) { ClearIngredients(); }

    private IEnumerator SpawnIngredients()
    {
        // Spawns ingredients until combat is over
        while (!isBattleOver)
        {

            // Waits a specified duration before spawning a new ingredient
            yield return new WaitForSeconds(ingerientSpawnInterval);
            //Debug.Log("Spawning an ingredient");


            // Determines position within circle for ingredient
            float x_Pos = UnityEngine.Random.Range(-18f, 18f);
            float z_Pos = UnityEngine.Random.Range(-18f, 18f);

            while (Vector2.Distance(new Vector2(x_Pos, z_Pos), new Vector2(0.0f, 0.0f)) > 18.0f) {
                x_Pos = UnityEngine.Random.Range(-18f, 18f);
                z_Pos = UnityEngine.Random.Range(-18f, 18f);
            }

            // Attempts to grab an ingredient from the pool
            GameObject temp = CombatObjectPool.Instance.GetPooledIngredient();

            // Ensure that pool was not maxed out
            if (temp != null)
            {
                // Spawns ingredient, assigns location, time til despawn, and color from available pool
                temp.GetComponentInChildren<IngredientScript>().ingredient = spawnawbleIngredients[UnityEngine.Random.Range(0, spawnawbleIngredients.Length)];

                temp.GetComponentInChildren<VisualEffect>().SetFloat("Lifetime", ingerientDespawnTime);
                temp.GetComponentInChildren<VisualEffect>().SetVector4("IngredientColor", temp.GetComponentInChildren<IngredientScript>().ingredient.color);

                temp.GetComponentInChildren<IngredientScript>().despawnTime = ingerientDespawnTime;

                temp.transform.position = new Vector3(x_Pos, 0f, z_Pos);

                temp.SetActive(true);
            }
        }

    }

    public void ProcessEnemyDeaths()
    {
        List<GameObject> deadEnemies = new List<GameObject>();

        foreach (GameObject enemy in enemiesInCombat)
        {
            EnemyStats stats = enemy.GetComponent<EnemyStats>();

            if (stats != null && stats.getHealth() <= 0)
            {
                deadEnemies.Add(enemy);
            }
        }

        foreach (GameObject enemy in deadEnemies)
        {
            enemiesInCombat.Remove(enemy);
            Destroy(enemy);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        Debug.Log("I have been passed " + enemy.name + " to remove!");

        // Grants experience based on enemy level disparity
        float levelMod = ((enemy.GetComponent<EnemyStats>().getLevel() - player.GetComponent<PlayerStats>().getLevel()) * .05f) + 1;
        int trueExp = Mathf.FloorToInt(enemy.GetComponent<EnemyStats>().getExp() * levelMod);
        if (trueExp > 0)
        {
            experienceEarned += trueExp;
            Debug.Log("You earned " + trueExp + " experience!");
        } else
        {
            experienceEarned += 1;
            Debug.Log("You earned " + 1 + " experience!");
        }

        //enemiesInCombat.Remove(enemy);
    }

    private string CalculateIngredients()
    {
        // Print list of ingredients
        Debug.Log("OLD LIST");
        foreach (CombatIngredient ci in collectedIngredients)
        {
            Debug.Log(ci.ingredientName);
        }

        // Sorts ingredients
        collectedIngredients.Sort((a, b) => a.ingredientPriority.CompareTo(b.ingredientPriority));

        string temp = "";

        // Print list of ingredients
        Debug.Log("NEW LIST");
        foreach (CombatIngredient ci in collectedIngredients)
        {
            Debug.Log(ci.ingredientName);
            temp += ci.ingredientPriority;
        }

        return temp;
    }

    public int CalculateDamage(IDamagable attacker, IDamagable defender, int basePower)
    {
        //float levelMod = ((player.GetComponent<PlayerStats>().getLevel() - enemy.GetComponent<EnemyStats>().getLevel()) * .05f) + 1;
        float attackerAttack = attacker.Stats.Attack;
        float defenderDefense = defender.Stats.Defense;

        //Debug.Log("Level Mod: " + levelMod);
        //Debug.Log("Player Attack: " + playerAttack);
        //Debug.Log("Enemy Def: " + enemyDefense);


        return Mathf.RoundToInt((Mathf.Pow(attackerAttack, 1.2f) / (defenderDefense + 20f)) * basePower * 1) + 1;
    }

    public int GetCollectedIngredientCount()
    {
        return collectedIngredients.Count;
    }

    public GameObject GetPlayerGameObject()
    {
        return player;
    }

    public int GetEnemyCount()
    {
        return enemiesInCombat.Count;
    }

    // VICTORY STAT SETTERS

    public void increaseDamageDealt(int damage)
    {
        damageDealt += damage;
    }

    public void increaseDamageTaken(int damage)
    {
        damageTaken += damage;
    }

    private IEnumerator increaseTimeTaken()
    {
        while(!isBattleOver)
        {
            yield return new WaitForSeconds(1f);
            timeTaken++;
        }
    }
}
