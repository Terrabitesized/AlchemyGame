using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerStats : MonoBehaviour, IDamagable
{
    readonly List<IEffect<IDamagable>> activeEffects = new();

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int playerAttack;
    [SerializeField] private int playerDefense;
    [SerializeField] private int playerLevel;

    [SerializeField] GameObject castingVFX;
    [SerializeField] PlayerHealthBar healthBar;
    private CombatManager cm;
   
    public void Awake()
    {
        
        healthBar = FindFirstObjectByType<PlayerHealthBar>();

        health = StaticCombatData.maxHealth;
        maxHealth = StaticCombatData.maxHealth;
        playerAttack = StaticCombatData.playerAttack;
        playerDefense = StaticCombatData.playerDefense;
        playerLevel = StaticCombatData.playerLevel;

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

    }

    public void ApplyEffect(IEffect<IDamagable> effect)
    {
        effect.OnCompleted += RemoveEffect;
        activeEffects.Add(effect);
        effect.Apply(this);
    }

    void RemoveEffect(IEffect<IDamagable> effect)
    {
        effect.OnCompleted -= RemoveEffect;
        activeEffects.Remove(effect);
    }

    bool IDamagable.takeDamage(int damage)
    {
        if (damage > 0)
        {
            cm.increaseDamageTaken(damage);
        }
        
        setHP(health - damage);
        return false;
    }

    public void setHP(int newHealth)
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

    public void PlayCastingEffect()
    {
        StartCoroutine(PlayCastingEffectCoroutine());
    }

    private IEnumerator PlayCastingEffectCoroutine()
    {
        if(castingVFX.activeSelf)
        {
            castingVFX.SetActive(false);
        }

        castingVFX.SetActive(true);
        yield return new WaitForSeconds(castingVFX.GetComponent<VisualEffect>().GetFloat("Duration") + .5f);
        castingVFX.SetActive(false);
    }
}
