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

    [Header("Dynamic Combat Variables")]
    [SerializeField] private CombatIngredient[] collectedIngredients;
    [SerializeField] private GameObject[] enemiesInCombat;
    private int numOfIngredients = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

            // Call function to brew potion
            // Clear collectedIngredients
            // Set numOfIngredients to 0.
            ClearIngredients();
        }
        
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
            Debug.Log("Spawning an ingredient");


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
