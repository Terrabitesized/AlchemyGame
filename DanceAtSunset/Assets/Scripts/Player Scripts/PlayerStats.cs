using Mono.Cecil;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;


    void Start()
    {
        
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
        Debug.Log("Health: " + health);
    }
}
