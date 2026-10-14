using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamagable
{
    public static event Action<int, IDamagable> OnEnemyDamaged;
    public static event Action<int, IDamagable> OnEnemyShieldDamaged;

    readonly List<IEffect<IDamagable>> activeEffects = new();

    public EnemyType EnemyType;
    public Stats Stats { get; set; }
    [SerializeField] private BaseStats baseStats;
    [SerializeField] private int experience;

    [SerializeField] private EnemyHealthbar healthBar;
    private CombatManager combatManager;
    private DamagePopupGenerator damagePopupGenerator;

    private void Awake()
    {
        Stats = new Stats(new StatsMediator(), baseStats);
        Stats.Shield = Stats.MaxHealth;

        healthBar = GetComponentInChildren<EnemyHealthbar>();
        damagePopupGenerator = GetComponent<DamagePopupGenerator>();
    }

    private void Start()
    {
        healthBar.UpdateHealthBar(Stats.CurrentHealth, Stats.MaxHealth);
        healthBar.UpdateShieldBar(Stats.Shield, Stats.MaxHealth);

        combatManager = CombatManager.Instance;
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

    private void ProcessDamage(int damage)
    {
        // Check for shield
        if (Stats.Shield > 0)
        {
            // Subtract from shield until shield breaks or all damage is absorbed
            int overflowDamage = damage - Stats.Shield;
            int remainingShield = Stats.Shield;

            if (overflowDamage > 0)
            {
                OnEnemyShieldDamaged?.Invoke(remainingShield, this);
                SetShield(0);

                OnEnemyDamaged?.Invoke(Stats.CurrentHealth - overflowDamage, this);
                SetHealth(Stats.CurrentHealth - overflowDamage);
            }
            else
            {
                SetShield(Stats.Shield - damage);
                OnEnemyShieldDamaged?.Invoke(damage, this);
            }
        }
        else
        {
            OnEnemyDamaged?.Invoke(damage, this);
            SetHealth(Stats.CurrentHealth - damage);
        }
    }

    public void SetHealth(int newHealth)
    {
        Stats.CurrentHealth = newHealth;
        healthBar.UpdateHealthBar(Stats.CurrentHealth, Stats.MaxHealth);

        // Enemy has died
        if (Stats.CurrentHealth <= 0)
        {
            ProccessDeath();
            return;
        }
    }

    public void SetShield(int newShield)
    {
        Stats.Shield = newShield;
        healthBar.UpdateShieldBar(Stats.Shield, Stats.MaxHealth);
    }

    public void TakeDamage(int basePower, Stats attackerStats)
    {
        int damage = CombatManager.Instance.CalculateDamage(attackerStats, Stats, basePower);
        ProcessDamage(damage);
    }

    private void ProccessDeath()
    {
        Debug.Log("This " + gameObject.name + " enemy has died!");

        combatManager.RemoveEnemy(this.gameObject);

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];
            effect.OnCompleted -= RemoveEffect;
            effect.Cancel();
        }

        QuestManager.Instance?.HandleEvent(new EnemyKilledEvent(EnemyType));

        activeEffects.Clear();
    }

    public int GetExperience() { return experience; }
}
