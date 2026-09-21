using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldEnemySpawning : MonoBehaviour
{
    public static OverworldEnemySpawning Instance;

    [Header("Spawning Variables")]
    [SerializeField] private Vector3[] spawnLocations;
    [SerializeField] private bool[] enemyInLocation;
    [SerializeField] private float spawnTime;
    [SerializeField] private float gracePeriod;
    [SerializeField] private bool enemiesSpawning = false;

    [Header("Enemy Variety & Weight")]
    [SerializeField] private GameObject[] spawnableEnemyPrefabs;
    [SerializeField] private GameObject[] spawnableEnemyData;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyInLocation = new bool[spawnLocations.Length];
        StartCoroutine(SpawnEnemies());
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

                // Add to list of all enemies
                spawnedEnemies.Add(temp);

                // Get the roaming enemy component
                RoamingEnemy roamingEnemy = temp.GetComponent<RoamingEnemy>();

                // Set its actual spawn/home position
                roamingEnemy.SetHomePoint(spawnLocations[spawnIndex]);

                // Picks enemy to spawn
                int dataIndex = Random.Range(0, spawnableEnemyData.Length);
                GameObject selectedData = spawnableEnemyData[dataIndex];

                // Determines how many enemies can be in a single prefab
                int amountOfEnemies = Random.Range(1, 4);

                for(int i = 0; i < amountOfEnemies; i++)
                {
                    temp.GetComponent<RoamingEnemy>().enemies.Add(selectedData);
                }
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }

    /// <summary>
    /// Despawns all enemies.
    /// </summary>
    /// <param name="disableSpawning">If set to false, enemies will stop spawning. If set to true,
    /// enemies will continue to spawn after despawning all current enemies.</param>

    public void DespawnAllEnemies(bool disableSpawning)
    {
        enemiesSpawning = disableSpawning;

        for(int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject temp = spawnedEnemies[i];

            spawnedEnemies.Remove(temp);
            Destroy(temp);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        foreach(Vector3 vec in spawnLocations)
        {
            Gizmos.DrawWireSphere(vec, 1f);
            Gizmos.DrawIcon(vec, "SpawnZone");
        }
    }
}
