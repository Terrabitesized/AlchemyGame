using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamagable
{
    readonly List<IEffect<IDamagable>> activeEffects = new();

    [SerializeField] BaseStats baseStats;
    public Stats Stats { get; set; }

    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attack = 0;
    [SerializeField] private int defense = 0;
    [SerializeField] private int level = 0;
    [SerializeField] private int exp = 0;

    public static event Action<int, IDamagable> OnEnemyDamaged;

    [SerializeField] EnemyHealthbar healthBar;
    private CombatManager combatManager;
    private DamagePopupGenerator damagePopupGenerator;

    private void Awake()
    {
        Stats = new Stats(new StatsMediator(), baseStats);

        health = baseStats.maxHealth;
        maxHealth = baseStats.maxHealth;
        attack = baseStats.attack;
        defense = baseStats.defense;
        level = baseStats.level;

        healthBar = GetComponentInChildren<EnemyHealthbar>();
        damagePopupGenerator = GetComponent<DamagePopupGenerator>();
    }

    private void Start()
    {
        healthBar.UpdateHealthBar(health, maxHealth);

        combatManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            combatManager.ProcessEnemyDeaths();
        }
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

    //public bool takeDamage(int damage)
    //{
    //    damagePopupGenerator.CreatePopUp(transform.position, "" + damage);
    //    return setHP(health - damage);
    //}

    public bool setHP(int newHealth)
    {
        
        health = newHealth;

        // Enemy has died
        if (health <= 0)
        {
            Debug.Log("This " + gameObject.name + " enemy has died!");

            combatManager.RemoveEnemy(this.gameObject);

            //Destroy(this.gameObject);
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
        healthBar.UpdateHealthBar(health, maxHealth);

        return true;
    }

    public int getHealth()
    {
        return health;
    }

    public int getAttack()
    {
        return attack;
    }

    public int getDefense()
    {
        return defense;
    }

    public int getLevel()
    {
        return level;
    }

    public int getExp()
    {
        return exp;
    }

    bool IDamagable.takeDamage(int basePower, Stats attackerStats)
    {
        int damage = CombatManager.Instance.CalculateDamage(attackerStats, Stats, basePower);

        OnEnemyDamaged?.Invoke(damage, this);
        return setHP(health - damage);
    }
}
