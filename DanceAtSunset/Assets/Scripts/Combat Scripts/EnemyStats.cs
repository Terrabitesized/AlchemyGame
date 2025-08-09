using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attack = 0;
    [SerializeField] private int defense = 0;
    [SerializeField] private int level = 0;
    [SerializeField] private int exp = 0;


    [SerializeField] EnemyHealthbar healthBar;
    private CombatManager combatManager;
    private DamagePopupGenerator damagePopupGenerator;

    private void Awake()
    {
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
            takeDamage(10);
    }

    public void takeDamage(int damage)
    {
        damagePopupGenerator.CreatePopUp(transform.position, "" + damage);
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
}
