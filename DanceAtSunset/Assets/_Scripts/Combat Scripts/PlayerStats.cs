using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerStats : MonoBehaviour, IDamagable
{
    public static Action<int, IDamagable> OnPlayerDamaged;

    readonly List<IEffect<IDamagable>> activeEffects = new();
    public Stats Stats { get; set; }
    private BaseStats baseStats;

    [SerializeField] private int currentHealth;
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
        // Set baseStats
        if(StaticCombatData.BaseStats != null)
            baseStats = StaticCombatData.BaseStats;

        Stats = new Stats(new StatsMediator(), baseStats);

        healthBar = FindFirstObjectByType<PlayerHealthBar>();

        currentHealth = baseStats.currentHealth;
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
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }


    void Update()
    {
        Stats.Mediator.Update(Time.deltaTime);
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

        if (basePower > 0)
            cm.IncreaseDamageTaken(damage);
        else
        {
            // Negative base power yields healing
            SetHealth(currentHealth - basePower);
            OnPlayerDamaged?.Invoke(basePower, this);
            return false;
        }

        SetHealth(currentHealth - damage);
        OnPlayerDamaged?.Invoke(damage, this);
        return false;
    }

    public void SetHealth(int newHealth)
    {
        currentHealth = newHealth;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        Debug.Log("Current Health: " + currentHealth);
    }

    public int getHP()
    {
        return currentHealth;
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
        if(spell.spellAbility.requiresCasting)
        {
            castingEffectCoroutine = StartCoroutine(PlayCastingEffectCoroutine(spell.spellAbility.castDuration));

            // Enable and Init popup
            abilityPopupAnimator?.gameObject.SetActive(true);
            abilityPopupAnimator?.Init(spell.spellAbility.castDuration, spell.spellName);
        }
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
