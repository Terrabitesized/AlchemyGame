using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get reference to player
        player = GameObject.FindGameObjectWithTag("Player");

        // Check if the player is loading from combat
        if(StaticOverworldData.loadingFromCombat)
        {
            Debug.Log("WE ARE LOADING FROM COMBAT");

            StaticOverworldData.loadingFromCombat = false;

            if (player != null)
            {
                player.transform.position = StaticOverworldData.playerPosition;
                player.transform.rotation = StaticOverworldData.playerRotation;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
