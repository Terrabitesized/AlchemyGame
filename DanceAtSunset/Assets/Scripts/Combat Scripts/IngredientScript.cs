using UnityEngine;

public class IngredientScript : MonoBehaviour
{
    public float despawnTime = 5f;
    public CombatIngredient ingredient;
    private CombatManager cm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cm = GameObject.FindWithTag("GameController").GetComponent<CombatManager>();

        Destroy(this.gameObject, despawnTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Ingredient Collision");

            // Send data to Combat Manager, and destroy self
            cm.AddIngredient(ingredient);

            Destroy(this.gameObject);
        }
    }
}
