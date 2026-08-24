using System;
using System.Collections;
using UnityEngine;

public class IngredientScript : MonoBehaviour
{
    public float spawnBufferTime = .5f;
    public float despawnTime = 5f;
    public CombatIngredient ingredient;

    private CombatManager cm;
    private bool canBePickedup = false;

    public static Action<CombatIngredient> OnIngredientCollected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cm = GameObject.FindWithTag("GameController").GetComponent<CombatManager>();

        Invoke("EnableSelf", spawnBufferTime);
        Invoke("DisableSelf", despawnTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && canBePickedup)
        {
            //Debug.Log("Ingredient Collision");

            // Send data to Combat Manager, and disable self
            if (cm.GetCollectedIngredientCount() < 3)
            {
                OnIngredientCollected?.Invoke(ingredient);
                cm.AddIngredient(ingredient);
            }
            else
            {
                return;
            }

            DisableSelf();
        }
    }

    void EnableSelf()
    {
        canBePickedup = true;
    }

    void DisableSelf()
    {
        this.transform.parent.gameObject.SetActive(false);
    }
}
