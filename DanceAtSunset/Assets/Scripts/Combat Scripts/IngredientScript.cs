using UnityEngine;

public class IngredientScript : MonoBehaviour
{
    public float despawnTime = 5f;
    public CombatIngredient ingredient;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, despawnTime);
    }
}
