using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class OverworldEnemySpawning : MonoBehaviour
{
    [Header("Spawning Variables")]
    [SerializeField] private Vector3[] spawnLocations;
    [SerializeField] private bool[] enemyInLocation;
    [SerializeField] private float spawnTime;
    [SerializeField] private float gracePeriod;
    [SerializeField] private bool enemiesSpawning = false;

    [Header("Enemy Variety & Weight")]
    [SerializeField] private GameObject[] spawnableEnemyPrefabs;
    [SerializeField] private GameObject[] spawnableEnemyData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyInLocation = new bool[spawnLocations.Length];
        StartCoroutine(SpawnEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnEnemies()
    {
        // Waits for grace period before spawning enemies
        yield return new WaitForSeconds(gracePeriod);

        enemiesSpawning = true;

        while(enemiesSpawning)
        {
            // Finds a random location and attempts to spawn enemy at it
            int spawnIndex = Random.Range(0, spawnLocations.Length);

            if(!enemyInLocation[spawnIndex])
            {
                // Marks this location as having an enemy
                enemyInLocation[spawnIndex] = true;

                // Picks a random enemy to spawn
                int enemyIndex = Random.Range(0, spawnableEnemyPrefabs.Length);
                GameObject temp = Instantiate(spawnableEnemyPrefabs[enemyIndex]);

                // Sets enemy location
                temp.transform.position = spawnLocations[spawnIndex];

                // Picks enemy to spawn
                int dataIndex = Random.Range(0, spawnableEnemyData.Length);
                GameObject selectedData = spawnableEnemyData[dataIndex];

                // Determines how many enemies can be in a single prefab
                int amountOfEnemies = Random.Range(0, 3);

                for(int i = 0; i < amountOfEnemies; i++)
                {
                    temp.GetComponent<RoamingEnemy>().enemies.Add(selectedData);
                }
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }
}
