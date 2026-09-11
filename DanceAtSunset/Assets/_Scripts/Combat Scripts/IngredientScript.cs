using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientScript : MonoBehaviour
{
    public static Action<CombatIngredient> OnIngredientCollected;
    [SerializeField] private GameObject subIconHolder;
    [SerializeField] private Image subIcon;

    public float spawnBufferTime = .5f;
    public float despawnTime = 5f;
    public CombatIngredient ingredient;
    public Spell defaultAbility;
    public List<Ability> subAbilities = new List<Ability>();

    private CombatManager cm;
    private bool canBePickedup = false;
    private Camera cam;

    private Coroutine enableCoroutine = null;
    private Coroutine disableCoroutine = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cm = GameObject.FindWithTag("GameController").GetComponent<CombatManager>();
    }

    private void OnEnable()
    {
        cam = Camera.main;

        subAbilities.Clear();
        subIcon.CrossFadeAlpha(1f, 0f, true); // Set sub icon to visible

        enableCoroutine = StartCoroutine(EnableSelf(spawnBufferTime));
        disableCoroutine = StartCoroutine(DisableSelf(despawnTime));
    }

    private void Update()
    {
        if (CombatManager.Instance.isBattleOver)
            return;

        subIconHolder.transform.rotation = Quaternion.LookRotation(
            cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && canBePickedup)
        {
            // Send data to Combat Manager, and disable self
            if (cm.GetCollectedIngredientCount() < 3)
            {
                OnIngredientCollected?.Invoke(ingredient);
                cm.AddIngredient(ingredient);

                // Execute main ability
                if(defaultAbility != null)
                    defaultAbility.spellAbility.Target(PotionManager.Instance.targetingManager, other.GetComponent<IDamagable>());

                // Execute all sub abilities
                foreach (Ability a in subAbilities)
                    a.Target(PotionManager.Instance.targetingManager, other.GetComponent<IDamagable>());
            }
            else
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(DisableSelf(0));
        }
    }

    public void SetSubIcon(int index)
    {
        switch (index)
        {
            case 0: // Attack
                break;
            case 1: // Defense
                break;
            case 2: // Heal
                break;
            default:
                subIcon.CrossFadeAlpha(0f, 0f, true); // Set sub icon invisible
                break;
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
