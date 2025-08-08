using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class RoamingEnemy : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

        SceneManager.LoadScene("CombatTestScene");
    }
}
