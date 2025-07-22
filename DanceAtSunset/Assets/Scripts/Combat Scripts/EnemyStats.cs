using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int damage)
    {
        setHP(health - damage);
    }

    public void setHP(int newHealth)
    {
        health = newHealth;
        Debug.Log("Health: " + health);
    }
}
