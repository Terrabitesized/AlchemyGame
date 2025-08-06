using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TestAI1 : MonoBehaviour
{
    public Vector3[] PuddlePositions;

    private Vector3 targetPos;

   
    //bool onCooldown = false;
    bool alive = true;
    [SerializeField] private float atkCooldown = 5f;

    [Header("Attack 1 Variables")]
    [SerializeField] private GameObject hurtPuddle;
    [SerializeField] private GameObject warnPuddle;

    [SerializeField] private float atkWaitTime1 = 0.5f;

    [SerializeField] private int hurtPuddleDamage = 5;
    [SerializeField] private float hurtPuddleDamageCooldown = .5f;

    [Header("Attack 2 Variables")]
    [SerializeField] private GameObject hurtArea;
    [SerializeField] private GameObject warnArea;

    private Vector3 savedPos;

    [SerializeField] private float atkWaitTime2 = 0.5f;


    [SerializeField] private int hurtAreaDamage = 5;

    private void Awake()
    {
        
    }

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
                slimeAttack1();
            } else if (atkChoice == 2)
            {
                slimeAttack2();
            }
        }
    }

    // ATTACK 1 START

    private void slimeAttack1()
    {

        PuddlePositions = new Vector3[10];

        for (int i = 0; i < 10; i++)
        {
            float x_Pos = Random.Range(-18f, 18f);
            float z_Pos = Random.Range(-18f, 18f);

            while (Vector2.Distance(new Vector2(x_Pos, z_Pos), new Vector2(0.0f, 0.0f)) > 18.0f)
            {
                x_Pos = Random.Range(-18f, 18f);
                z_Pos = Random.Range(-18f, 18f);
            }

            PuddlePositions[i] = new Vector3(x_Pos, 0, z_Pos);

            GameObject temp = Instantiate(warnPuddle);
            
            temp.transform.position = new Vector3(x_Pos, 0, z_Pos);


        }
        StartCoroutine("placeAttack1");
    }

    private IEnumerator placeAttack1()
    {
        yield return new WaitForSeconds(atkWaitTime1);

       

        for (int i = 0; i < 10; i++)
        {
        GameObject temp = Instantiate(hurtPuddle);


        temp.GetComponent<AttackAttributes>().damage = hurtPuddleDamage;
        temp.GetComponent<AttackAttributes>().damageCooldown = hurtPuddleDamageCooldown;

       
        temp.transform.position = new Vector3(PuddlePositions[i].x, 0, PuddlePositions[i].z);
        }
    }

    // ATTACK 1 END

    // ATTACK 2 START

    private void slimeAttack2()
    {
        GameObject temp = Instantiate(warnArea);

        targetPos = GameObject.FindWithTag("Player").transform.position;

        savedPos = new Vector3(targetPos.x, 0, targetPos.z);

        temp.transform.position = savedPos;

        StartCoroutine("placeAttack2");
    }

    private IEnumerator placeAttack2()
    {
        yield return new WaitForSeconds(atkWaitTime2);

        GameObject temp = Instantiate(hurtArea);


        temp.GetComponent<AttackAttributes>().damage = hurtAreaDamage;
        temp.GetComponent<AttackAttributes>().damageCooldown = 5;

        temp.transform.position = savedPos;
    }



}
