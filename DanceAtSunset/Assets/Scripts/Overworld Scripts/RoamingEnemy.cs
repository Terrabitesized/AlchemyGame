using UnityEngine;
using UnityEngine.SceneManagement;

public class RoamingEnemy : MonoBehaviour
{
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
            CombatSetup();
        }
    }

    void CombatSetup()
    {
        StaticCombatData.message = "Balls";
        SceneManager.LoadScene("CombatTestScene");
    }
}
