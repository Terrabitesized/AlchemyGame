using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> enemiesInCombat;

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
                Debug.Log("a is a string");
                break;
            case "002": // Red, Red, Green
                RedRedRed();
                break;
            case "011": // Red, Blue, Blue
                Debug.Log("a is a string");
                break;
            case "012": // Red, Blue, Green
                RedRedRed();
                break;
            case "022": // Red, Green, Green
                Debug.Log("a is a string");
                break;
            case "111": // Blue, Blue, Blue
                Debug.Log("a is a string");
                break;
            case "112": // Blue, Blue, Green
                Debug.Log("a is a string");
                break;
            case "122": // Blue, Green, Green
                Debug.Log("a is a string");
                break;
            case "222": // Green, Green, Green
                Debug.Log("a is a string");
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
}
