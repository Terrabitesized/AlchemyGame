using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public bool isBattleOver = false;
    [SerializeField] private GameObject ingredient;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("SpawnIngredients");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnIngredients()
    {
        while (!isBattleOver)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Spawning an ingredient");

            float x_Pos = Random.Range(-15f, 15f);
            float z_Pos = Random.Range(-15f, 15f);

            GameObject temp = Instantiate(ingredient);
            temp.transform.position = new Vector3(x_Pos, 2f, z_Pos);

        }

    }
}
