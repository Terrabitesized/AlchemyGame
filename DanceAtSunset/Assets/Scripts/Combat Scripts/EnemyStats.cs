using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] EnemyHealthbar healthBar;
    private CombatManager combatManager;

    private void Awake()
    {
        healthBar = GetComponentInChildren<EnemyHealthbar>();
    }

    private void Start()
    {
        healthBar.UpdateHealthBar(health, maxHealth);

        combatManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();
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

        // Enemy has died
        if (health <= 0)
        {
            Debug.Log("This " + gameObject.name + " enemy has died!");

            combatManager.RemoveEnemy(this.gameObject);

            Destroy(this.gameObject);
        }

        // Update's enemy health bar UI
        healthBar.UpdateHealthBar(health, maxHealth);
    }
}
