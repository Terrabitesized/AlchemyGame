using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class RoamingEnemy : MonoBehaviour
{
    [Header("Important Vars")]
    public List<GameObject> enemies;
    [SerializeField] private Transform playerPos;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] private float waitTime = 2f; // Seconds to wait at each patrol point
    [SerializeField] private float maxChaseDistance = 15f; // Max distance from home before giving up
    private bool waiting = false;
    private float waitTimer = 0f;
    private bool returningHome = false;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange;
    [SerializeField] private Vector3 homePoint;

    [Header("States")]
    [SerializeField] private float sightRange;
    private bool playerInSightRange;

    // AI STUFF

    private void Awake()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        homePoint = transform.position; // Start patrol area here
    }

    void Update()
    {
        if (!enabled) return;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (returningHome)
        {
            ReturnHome();
            return; 
        }

        if (playerInSightRange)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
    }

    private void Patroling()
    {
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                walkPointSet = false;
            }
            return;
        }

        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(walkPoint);

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                StartWaiting();
            }
        }
    }

    private void StartWaiting()
    {
        waiting = true;
        waitTimer = waitTime;
        agent.ResetPath(); // Stop moving during wait
    }

    private void SearchWalkPoint()
    {
        // Pick a random direction around the home point
        Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;
        randomDirection += homePoint; // Use homePoint to keep enemy in area

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkPointRange, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void Chasing()
    {
        // If too far from home, start returning and stop chasing
        float distFromHome = Vector3.Distance(transform.position, homePoint);
        if (distFromHome > maxChaseDistance)
        {
            returningHome = true;
            agent.SetDestination(homePoint);
            return;
        }

        agent.SetDestination(playerPos.position);
    }

    private void ReturnHome()
    {
        agent.SetDestination(homePoint);

        if (!agent.pathPending && agent.remainingDistance < maxChaseDistance - 5)
        {
            returningHome = false;
            walkPointSet = false; // Go back to patrolling
        }
    }


    // NON AI STUFF

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CombatSetup(other);
        }
    }

    void CombatSetup(Collider player)
    {
        
       

        // Load data based on player stats and specific enemy hit
        StaticCombatData.message = "Balls";
        StaticCombatData.enemies = enemies;

        StaticCombatData.playerAttack = player.gameObject.GetComponent<OverworldStats>().getAtk();
        StaticCombatData.playerDefense = player.gameObject.GetComponent<OverworldStats>().getDef();
        StaticCombatData.playerLevel = player.gameObject.GetComponent<OverworldStats>().getLevel();
        StaticCombatData.maxHealth = player.gameObject.GetComponent<OverworldStats>().getMaxHp();
        StaticCombatData.currentExp = player.gameObject.GetComponent<OverworldStats>().getExp();

        StaticOverworldData.playerPosition = player.transform.position;
        StaticOverworldData.playerRotation = player.transform.rotation;

        if (ScreenShatter.Instance != null)
            StartCoroutine(ScreenShatter.Instance.TakeScreenshot());

        SceneManager.LoadScene("CombatTestScene");
    }
}
