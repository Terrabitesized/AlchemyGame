using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class CombatManager : MonoBehaviour
{
    public bool isBattleOver = false;

    [Header("Inherited Variables")]
    [SerializeField] private float ingerientSpawnInterval = .5f;
    [SerializeField] private float ingerientDespawnTime = 5f;
    [SerializeField] private CombatIngredient[] spawnawbleIngredients;
    [SerializeField] private GameObject ingredientModel;
    [SerializeField] private TextMeshProUGUI subtitles;

    [SerializeField] private GameObject playerPrefab;

    [Header("Dynamic Combat Variables")]
    [SerializeField] private CombatIngredient[] collectedIngredients;
    [SerializeField] private GameObject[] enemiesInCombat;
    private int numOfIngredients = 0;

    private void Awake()
    {
        // Spawn the player's prefav, will need loaded stats at a later point
        GameObject playPrefab = Instantiate(playerPrefab);
        playPrefab.transform.position = new Vector3(0f, 1.12f, -10f);
    }

    void Start()
    {
        // Spawn enemies based on what enemies were in spawned roamable in overworld
        if(enemiesInCombat.Length > 0)
        {
            // Spawn the only enemy in center
            if (enemiesInCombat.Length == 1)
            {
                GameObject temp = Instantiate(enemiesInCombat[0]);
                temp.transform.position = new Vector3(0f, 1f, 0f);

                // Reassign reference to clone, as to not modify prefab
                enemiesInCombat[0] = temp;
            } else {
                // Spawn enemies slightly spread apart
                for(int i = 0; i < enemiesInCombat.Length; i++)
                {
                    GameObject temp = Instantiate(enemiesInCombat[i]);
                    float x_Pos = Random.Range(-5f, 5f);
                    float z_Pos = Random.Range(-5f, 5f);
                    temp.transform.position = new Vector3(x_Pos, 1f, z_Pos);

                    // Reassign reference to clone, as to not modify prefab
                    enemiesInCombat[i] = temp;
                }
            }
        } else
        {
            // End combat
        }

        // Begin spawning Inregedients
        collectedIngredients = new CombatIngredient[3];
        StartCoroutine("SpawnIngredients");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Cleared potions");
            ClearIngredients();
        }
    }

    public void AddIngredient(CombatIngredient ing)
    {
        collectedIngredients[numOfIngredients] = ing;
        

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
            // Do potion thing based on ingredients
            ///////////////////////////////////////
            CalculateIngredients();


            // Call function to brew potion
            // Clear collectedIngredients
            // Set numOfIngredients to 0.
            ClearIngredients();
        }
        
    }

    void CalculateIngredients()
    {
        // Damage enemy
        int enemyTargetIndex = Random.Range(0, enemiesInCombat.Length);
        Debug.Log(enemyTargetIndex);

        enemiesInCombat[enemyTargetIndex].GetComponent<EnemyStats>().takeDamage(5);
    }

    private void ClearIngredients()
    {
        for (int i = 0; i < 3; i++)
        {
            collectedIngredients[i] = null;
        }

        numOfIngredients = 0;
        subtitles.SetText("Empty");
    }

    private IEnumerator SpawnIngredients()
    {
        // Spawns ingredients until combat is over
        while (!isBattleOver)
        {

            // Waits a specified duration before spawning a new ingredient
            yield return new WaitForSeconds(ingerientSpawnInterval);
            //Debug.Log("Spawning an ingredient");


            // Determines position within circle for ingredient
            float x_Pos = Random.Range(-18f, 18f);
            float z_Pos = Random.Range(-18f, 18f);

            while (Vector2.Distance(new Vector2(x_Pos, z_Pos), new Vector2(0.0f, 0.0f)) > 18.0f) {
                x_Pos = Random.Range(-18f, 18f);
                z_Pos = Random.Range(-18f, 18f);
            }

            // Spawns ingredient, assigns location, time til despawn, and color from available pool
            GameObject temp = Instantiate(ingredientModel);

            temp.GetComponentInChildren<IngredientScript>().ingredient = spawnawbleIngredients[Random.Range(0, spawnawbleIngredients.Length)];

            temp.GetComponentInChildren<VisualEffect>().SetFloat("Lifetime", ingerientDespawnTime);
            temp.GetComponentInChildren<VisualEffect>().SetVector4("IngredientColor", temp.GetComponentInChildren<IngredientScript>().ingredient.color);

            temp.GetComponentInChildren<IngredientScript>().despawnTime = ingerientDespawnTime;

            temp.transform.position = new Vector3(x_Pos, 0f, z_Pos);

        }

    }
}
