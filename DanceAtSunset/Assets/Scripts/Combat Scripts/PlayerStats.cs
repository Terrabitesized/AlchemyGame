using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;

    [SerializeField] PlayerHealthBar healthBar;
   
    public void Awake()
    {
        
        healthBar = FindFirstObjectByType<PlayerHealthBar>();

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
}
