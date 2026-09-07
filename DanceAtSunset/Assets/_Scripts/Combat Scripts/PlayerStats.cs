using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerStats : MonoBehaviour, IDamagable
{
    readonly List<IEffect<IDamagable>> activeEffects = new();

    [SerializeField] BaseStats baseStats;
    public Stats Stats { get; set; }

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int playerAttack;
    [SerializeField] private int playerDefense;
    [SerializeField] private int playerLevel;

    [SerializeField] GameObject castingVFX;
    [SerializeField] PlayerHealthBar healthBar;
    [SerializeField] AbilityPopupAnimator abilityPopupAnimator;
    private Coroutine castingEffectCoroutine = null;

    private CombatManager cm;
   
    public void Awake()
    {
        Stats = new Stats(new StatsMediator(), baseStats);

        healthBar = FindFirstObjectByType<PlayerHealthBar>();

        health = baseStats.maxHealth;
        maxHealth = baseStats.maxHealth;
        playerAttack = baseStats.attack;
        playerDefense = baseStats.defense;
        playerLevel = baseStats.level;

        PotionManager.OnSpellCast += PlayCastingEffectAndPopup;
    }

    private void OnDestroy()
    {
        castingEffectCoroutine = null;
        PotionManager.OnSpellCast -= PlayCastingEffectAndPopup;
    }

    void Start()
    {
        cm = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }
    }


    void Update()
    {
        Stats.Mediator.Update(Time.deltaTime);
        //Debug.Log(Stats.ToString());
    }

    public void ApplyEffect(IEffect<IDamagable> effect, IDamagable attacker)
    {
        effect.OnCompleted += RemoveEffect;
        activeEffects.Add(effect);
        effect.Apply(this, attacker);
    }

    void RemoveEffect(IEffect<IDamagable> effect)
    {
        effect.OnCompleted -= RemoveEffect;
        activeEffects.Remove(effect);
    }

    bool IDamagable.takeDamage(int basePower, Stats attackerStats)
    {
        int damage = CombatManager.Instance.CalculateDamage(attackerStats, Stats, basePower);

        if (damage > 0)
        {
            cm.increaseDamageTaken(damage);
        }
        else
        {
            // Negative base power yields healing
            SetHealth(health - basePower);
            return false;
        }

        SetHealth(health - damage);
        return false;
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }
        Debug.Log("Health: " + health);
    }

    public int getHP()
    {
        return health;
    }

    public int getAttack()
    {
        return playerAttack;
    }

    public int getDefense()
    {
        return playerDefense;
    }

    public int getLevel()
    {
        return playerLevel;
    }

    public void setAttack(int newAttack)
    {
        playerAttack = newAttack;
    }

    public void setDefense(int newDefense)
    {
        playerDefense = newDefense;
    }

    public void setLevel(int newLevel)
    {
        playerLevel = newLevel;
    }

    public void PlayCastingEffectAndPopup(Spell spell)
    {
        castingEffectCoroutine = StartCoroutine(PlayCastingEffectCoroutine(spell.spellAbility.castDuration));

        // Enable and Init popup
        abilityPopupAnimator?.gameObject.SetActive(true);
        abilityPopupAnimator?.Init(spell.spellAbility.castDuration, spell.spellName);
    }

    private IEnumerator PlayCastingEffectCoroutine(float castDuration)
    {
        if(castingVFX.activeSelf)
        {
            castingVFX.SetActive(false);
        }

        // Slightly reduced to allow for particles to disolve
        castingVFX.GetComponent<VisualEffect>().SetFloat("Duration", castDuration * .9f);

        castingVFX.SetActive(true);
        yield return new WaitForSeconds(castDuration + 1f);
        castingVFX.SetActive(false);

        castingEffectCoroutine = null;
    }
}
