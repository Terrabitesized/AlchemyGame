using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> enemiesInCombat;
    private GameObject[] temp;

    public void SetupPM(GameObject p, List<GameObject> e)
    {
        player = p;
        enemiesInCombat = e;
    }

    public void ParseIngredients(string ing)
    {
        Debug.Log(ing);

        switch(ing)
        {
            case "000": // Red, Red, Red
                RedRedRed();
                break;
            case "001": // Red, Red, Blue
                RedRedBlue();
                break;
            case "002": // Red, Red, Green
                RedRedGreen();
                break;
            case "011": // Red, Blue, Blue
                
                break;
            case "012": // Red, Blue, Green
                
                break;
            case "022": // Red, Green, Green
                RedGreenGreen();
                break;
            case "111": // Blue, Blue, Blue
                
                break;
            case "112": // Blue, Blue, Green
                
                break;
            case "122": // Blue, Green, Green
                
                break;
            case "222": // Green, Green, Green
                
                break;
            default: // Something went wrong
                Debug.Log("INVALID INGREDIENT PATTERN FOUND: " + ing);
                break;
        }
    }

    // Deals large DMG to single enemy
    private void RedRedRed()
    {
        // Targets a random enemy on field
        int enemyTargetIndex = Random.Range(0, enemiesInCombat.Count);

        // Deals damage to targeted enemy
        // TODO: ADD DMG FORMULA BASED ON ENEMY DEF AND PLAYER ATK
        enemiesInCombat[enemyTargetIndex].GetComponent<EnemyStats>().takeDamage(50);
    }

    // Deals large DMG to single enemy
    private void RedRedBlue()
    {
        // Targets all enemies on field
        // TODO: ADD DMG FORMULA BASED ON ENEMY DEF AND PLAYER ATK
        temp = enemiesInCombat.ToArray();

        foreach (GameObject enemy in temp)
        {
            enemy.GetComponent<EnemyStats>().takeDamage(25);
        }
    }

    // Deals bounce DMG to random enemy(s)
    private void RedRedGreen()
    {
        temp = enemiesInCombat.ToArray();

        for (int i = 0; i < 3; i++)
        {
            // Targets a random enemy on field
            int enemyTargetIndex = Random.Range(0, temp.Length);

            // Deals damage to targeted enemy
            // TODO: ADD DMG FORMULA BASED ON ENEMY DEF AND PLAYER ATK
            temp[enemyTargetIndex].GetComponent<EnemyStats>().takeDamage(15);
        }
    }

    /**
     * 
     */



    // Small heal to self
    private void RedGreenGreen()
    {
        // Make scale off something later??
        player.GetComponent<PlayerStats>().takeDamage(-15);
    }
}
