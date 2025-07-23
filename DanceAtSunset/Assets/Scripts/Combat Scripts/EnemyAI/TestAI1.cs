using System.Collections;
using UnityEngine;

public class TestAI1 : MonoBehaviour
{

    //bool onCooldown = false;
    bool alive = true;
    [SerializeField] private float atkCooldown = 5f;

    [Header("Attack 1 Variables")]
    [SerializeField] private GameObject hurtPuddle;
    [SerializeField] private int hurtPuddleDamage = 5;
    [SerializeField] private float hurtPuddleDamageCooldown = .5f;



    void Start()
    {
        StartCoroutine("atkPlayer");
    }


    void Update()
    {

    }

    private IEnumerator atkPlayer()
    {
        while (alive)
        {
            yield return new WaitForSeconds(atkCooldown);

            int atkChoice = Random.Range(1, 3);

            // FIRST ATTACK
            // Multiple "corrosive puddles" are set around the arena.
            if (atkChoice == 1)
            {
                hurtPuddles();
            }
        }
    }

    private void hurtPuddles()
    {
        for (int i = 0; i < 10; i++)
        {
            float x_Pos = Random.Range(-18f, 18f);
            float z_Pos = Random.Range(-18f, 18f);

            while (Vector2.Distance(new Vector2(x_Pos, z_Pos), new Vector2(0.0f, 0.0f)) > 18.0f)
            {
                x_Pos = Random.Range(-18f, 18f);
                z_Pos = Random.Range(-18f, 18f);
            }

            GameObject temp = Instantiate(hurtPuddle);
            temp.GetComponent<AttackAttributes>().damage = hurtPuddleDamage;
            temp.GetComponent<AttackAttributes>().damageCooldown = hurtPuddleDamageCooldown;

            temp.transform.position = new Vector3(x_Pos, 0, z_Pos);
        }
    }

}
