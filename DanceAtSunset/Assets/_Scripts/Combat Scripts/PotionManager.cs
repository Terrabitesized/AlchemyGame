using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.VFX;

public enum IngredientType { Red, Green, Blue };
public class PotionManager : MonoBehaviour
{
    public static PotionManager Instance;

    [SerializeReference] public Spell[] potionSpells;
    private Dictionary<string, Spell> potionSpellRecipes = new Dictionary<string, Spell>();

    [SerializeReference] public TargetingManager targetingManager;

    [Header("Inherited Variables")]
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> enemiesInCombat;
    private GameObject[] temp;

    [Header("Other Variables")]
    [SerializeField] private float buffExtentionAmount = 15f;
    [SerializeField] private Spell currentSpell = null;
    private bool isCasting = false;

    private CombatManager cm;

    // Combat actions
    public static event Action<List<GameObject>> OnAttackBegin;
    public static event Action<List<GameObject>, List<bool>> OnAttackEnd;

    public static event Action<Spell> OnSpellPrimed; // When a spell is ready to be primed
    public static event Action<Spell> OnSpellCast; // Spell duration
    public static event Action OnSpellFail; // When an invalid spell is cast

    // DAMAGE FORMULA
    // (ATK ^ 1.2 / DEF + 20) * Level Mod * Base POW

    // Level Mod: 1 + ((PLV - ELV) * .05)
    // This grants +5% dmg for 1 level higher player
    private void Awake()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        // Reset important combat functions

    }

    private void Start()
    {
        cm = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();

        ExtactRecipes();
    }

    public void SetupPM(GameObject p, List<GameObject> e)
    {
        player = p;
        enemiesInCombat = e;
        targetingManager = player.GetComponent<TargetingManager>();
        targetingManager.potionManager = this;
    }

    public void ExtactRecipes()
    {
        // For each spell pre-populated into this combat
        foreach (Spell spell in potionSpells)
        {
            if (spell != null && spell.hasRecipe) // If the spell is not null & requires a recipe
            {
                // Create a string ID for that given spell recipe
                string recipe = "";
                bool validRecipe = true;
                foreach (IngredientType i in spell.castingRecipe)
                {
                    switch (i)
                    {
                        case IngredientType.Red:
                            recipe += "0";
                            break;
                        case IngredientType.Blue:
                            recipe += "1";
                            break;
                        case IngredientType.Green:
                            recipe += "2";
                            break;
                        default:
                            Debug.Log("Invalid IngredientType found!");
                            validRecipe = false;
                            break;
                    }
                }

                // If we have made a valid recipe, store it in the dictionary for later use
                // Spells with the same recipe will get skipped
                if (validRecipe)
                    potionSpellRecipes.TryAdd(recipe, spell);
            }
        }
    }

    /// <summary>
    /// Returns a List of IDamagable components for use by the TargetingManager
    /// </summary>
    public List<IDamagable> GetEnemiesInCombat()
    {
        List<IDamagable> result = new List<IDamagable>();
        foreach (GameObject p in enemiesInCombat)
        {
            if (p.TryGetComponent<IDamagable>(out var component))
                result.Add(component);
        }

        return result;
    }

    public void PrimeSpell(string ing)
    {
        if(potionSpellRecipes.ContainsKey(ing))
        {
            currentSpell = potionSpellRecipes[ing];
            OnSpellPrimed?.Invoke(currentSpell);
        }
        else
        {
            currentSpell = null;
            OnSpellPrimed?.Invoke(null);
        }
    }

    public void ResetCurrentSpell() { currentSpell = null; }

    public void CastCurrentSpell()
    {
        if (isCasting)
            return;

        if (currentSpell != null)
            StartCoroutine(PlayerBeginCast(currentSpell));
        else
            OnSpellFail?.Invoke();
    }

    public IEnumerator PlayerBeginCast(Spell spell)
    {
        yield return new WaitForEndOfFrame();

        var ability = spell.spellAbility;

        if (ability.requiresCasting)
        {
            isCasting = true;

            // Trigger the spell casting event
            OnSpellCast?.Invoke(spell);

            // Play casting SFX
            MusicManager.Instance.PlaySpellCast();

            // Wait until the cast duartion is up
            yield return new WaitForSeconds(ability.castDuration);

            // Execute the Ability
            ability.Target(targetingManager, player.GetComponent<IDamagable>());

            //cm.ProcessEnemyDeaths();

            // Wait a small amount longer to allow for visual effects to despawn
            yield return new WaitForSeconds(1f);

            isCasting = false;

            yield break;
        }

        // Execute the Ability
        ability.Target(targetingManager, player.GetComponent<IDamagable>());

        //cm.ProcessEnemyDeaths();
    }
}
