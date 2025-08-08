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
        healthBar.UpdateHealthBar(health, maxHealth);
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
        healthBar.UpdateHealthBar(health, maxHealth);
        Debug.Log("Health: " + health);
    }
}
