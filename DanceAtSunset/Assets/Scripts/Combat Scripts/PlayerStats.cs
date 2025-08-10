using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int playerAttack;
    [SerializeField] private int playerDefense;
    [SerializeField] private int playerLevel;

    [SerializeField] PlayerHealthBar healthBar;
   
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
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
            takeDamage(10);
    }

    public void takeDamage(int damage)
    {
        setHP(health - damage);    
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
}
