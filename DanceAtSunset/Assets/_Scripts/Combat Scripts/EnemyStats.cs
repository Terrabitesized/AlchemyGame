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

        healthBar = GetComponentInChildren<EnemyHealthbar>();
        damagePopupGenerator = GetComponent<DamagePopupGenerator>();
    }

    private void Start()
    {
        healthBar.UpdateHealthBar(Stats.CurrentHealth, Stats.MaxHealth);

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

    }

    public void SetHealth(int newHealth)
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

            QuestManager.Instance?.HandleEvent(new EnemyKilledEvent(EnemyType));

            activeEffects.Clear();

            return;
        }

        // Update's enemy health bar UI
        healthBar.UpdateHealthBar(Stats.CurrentHealth, Stats.MaxHealth);
    }

    public void TakeDamage(int basePower, Stats attackerStats)
    {
        int damage = CombatManager.Instance.CalculateDamage(attackerStats, Stats, basePower);

        // Check if shield is > 0
        if(Stats.Shield > 0)
        {

        }

        OnEnemyDamaged?.Invoke(damage, this);
        SetHealth(Stats.CurrentHealth - damage);
    }

    public int GetExperience() { return experience; }
}
