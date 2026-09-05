using System;
using System.Collections;
using UnityEngine;

public class IngredientScript : MonoBehaviour
{
    public static Action<CombatIngredient> OnIngredientCollected;

    public float spawnBufferTime = .5f;
    public float despawnTime = 5f;
    public CombatIngredient ingredient;

    private CombatManager cm;
    private bool canBePickedup = false;

    private Coroutine enableCoroutine = null;
    private Coroutine disableCoroutine = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cm = GameObject.FindWithTag("GameController").GetComponent<CombatManager>();
    }

    private void OnEnable()
    {
        enableCoroutine = StartCoroutine(EnableSelf(spawnBufferTime));
        disableCoroutine = StartCoroutine(DisableSelf(despawnTime));
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

            StopAllCoroutines();
            StartCoroutine(DisableSelf(0));
        }
    }

    private IEnumerator EnableSelf(float time)
    {
        yield return new WaitForSeconds(time);
        canBePickedup = true;
    }

    private IEnumerator DisableSelf(float time)
    {
        yield return new WaitForSeconds(time);
        this.transform.parent.gameObject.SetActive(false);
    }
}
