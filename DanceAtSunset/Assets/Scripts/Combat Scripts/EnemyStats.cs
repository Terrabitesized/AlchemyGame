using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;

    [SerializeField] EnemyHealthbar healthBar;

    private void Awake()
    {
        healthBar = GetComponentInChildren<EnemyHealthbar>();
    }

    private void Start()
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
        if (health <= 0)
        {

        }
        healthBar.UpdateHealthBar(health, maxHealth);
       // Debug.Log("Health: " + health);
    }
}
