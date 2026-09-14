using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamagable
{
    public static event Action<int, IDamagable> OnEnemyDamaged;

    readonly List<IEffect<IDamagable>> activeEffects = new();

    public Stats Stats { get; set; }
    [SerializeField] private BaseStats baseStats;
    [SerializeField] private int experience;

    [SerializeField] private EnemyHealthbar healthBar;
    private CombatManager combatManager;
    private DamagePopupGenerator damagePopupGenerator;

    private void Awake()
    {
        Stats = new Stats(new StatsMediator(), baseStats);

        healthBar = GetComponentInChildren<EnemyHealthbar>();
        damagePopupGenerator = GetComponent<DamagePopupGenerator>();
    }

    private void Start()
    {
        healthBar.UpdateHealthBar(Stats.CurrentHealth, Stats.MaxHealth);

        combatManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();
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

    public bool SetHealth(int newHealth)
    {

        Stats.CurrentHealth = newHealth;

        // Enemy has died
        if (Stats.CurrentHealth <= 0)
        {
            Debug.Log("This " + gameObject.name + " enemy has died!");

            combatManager.RemoveEnemy(this.gameObject);

            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = activeEffects[i];
                effect.OnCompleted -= RemoveEffect;
                effect.Cancel();
            }

            activeEffects.Clear();

            return false;
        }

        // Update's enemy health bar UI
        healthBar.UpdateHealthBar(Stats.CurrentHealth, Stats.MaxHealth);

        return true;
    }

    bool IDamagable.takeDamage(int basePower, Stats attackerStats)
    {
        int damage = CombatManager.Instance.CalculateDamage(attackerStats, Stats, basePower);

        OnEnemyDamaged?.Invoke(damage, this);
        return SetHealth(Stats.CurrentHealth - damage);
    }

    public int GetExperience() { return experience; }
}
