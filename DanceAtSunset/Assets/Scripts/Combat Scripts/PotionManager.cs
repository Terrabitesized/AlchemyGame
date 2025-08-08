using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    [Header("Inherited Variables")]
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> enemiesInCombat;
    private GameObject[] temp;

    [Header("Attack Buff Variables")]
    [SerializeField] private float attackBuffTimer = 0f;
    [SerializeField] private float attackBuffPercent = 1.3f;
    private bool attackIsBuffed = false;
    [SerializeField] private float attackBuffDuration = 30f;

    [Header("Defense Buff Variables")]
    [SerializeField] private float defenseBuffTimer = 0f;
    [SerializeField] private float defenseBuffPercent = 1.3f;
    private bool defenseIsBuffed = false;
    [SerializeField] private float defenseBuffDuration = 30f;

    [Header("Speed Buff Variables")]
    [SerializeField] private float speedBuffTimer = 0f;
    [SerializeField] private float speedBuffPercent = 1.3f;
    private bool speedIsBuffed = false;
    [SerializeField] private float speedBuffDuration = 30f;

    [Header("Other Variables")]
    [SerializeField] private float buffExtentionAmount = 15f;

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
                RedBlueBlue();
                break;
            case "012": // Red, Blue, Green
                RedBlueGreen();
                break;
            case "022": // Red, Green, Green
                RedGreenGreen();
                break;
            case "111": // Blue, Blue, Blue
                BlueBlueBlue();
                break;
            case "112": // Blue, Blue, Green
                BlueBlueGreen();
                break;
            case "122": // Blue, Green, Green
                BlueGreenGreen();
                break;
            case "222": // Green, Green, Green
                GreenGreenGreen();
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

    // Deals small DMG to all enemies
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

    // Buffs own attack
    private void RedBlueBlue()
    {
        // Sets buff duration to 10 seconds
        // Checks that current duration is not shorter than default,
        // due to extention effects from other combos
        if(attackBuffTimer < attackBuffDuration)
        {
            attackBuffTimer = attackBuffDuration;
        }

        // Avoids double calling coroutine, but will still reset buff duration
        // if reset while buff is active
        if(!attackIsBuffed)
        {
            StartCoroutine(AttackBuff(player));
        }
    }

    private IEnumerator AttackBuff(GameObject p)
    {
        // Maks coroutine as ongoing
        attackIsBuffed = true;

        // Retrieve old attack to restore once buff ends
        int oldAttack = p.GetComponent<PlayerStats>().getAttack();

        // Assigns new attack
        p.GetComponent<PlayerStats>().setAttack(Mathf.FloorToInt(oldAttack * attackBuffPercent));

        // Buff countdown, should allow for re-application with no issues, and cancelling of buffs
        while (attackBuffTimer > 0)
        {
            Debug.Log("Attack buff has: " + attackBuffTimer + " left.");
            yield return new WaitForSeconds(.1f);
            attackBuffTimer -= .1f;
        }

        // Resets attack
        p.GetComponent<PlayerStats>().setAttack(oldAttack);

        // Marks coroutine as having ended
        attackIsBuffed = false;
    }

    // Negates all buffs and debuffs
    private void RedBlueGreen()
    {
        attackBuffTimer = 0f;
        defenseBuffTimer = 0f;
    }

    // Small heal to self
    private void RedGreenGreen()
    {
        // Make scale off something later??
        player.GetComponent<PlayerStats>().takeDamage(-15);
    }

    // Extends all buffs
    private void BlueBlueBlue()
    {
        if(attackIsBuffed)
        {
            attackBuffTimer += buffExtentionAmount;
        }

        if (defenseIsBuffed)
        {
            defenseBuffTimer += buffExtentionAmount;
        }

        if (speedIsBuffed)
        {
            speedBuffTimer += buffExtentionAmount;
        }
    }

    // Buffs own defense
    private void BlueBlueGreen()
    {
        // Sets buff duration to 10 seconds
        // Checks that current duration is not shorter than default,
        // due to extention effects from other combos
        if (defenseBuffTimer < defenseBuffDuration)
        {
            defenseBuffTimer = defenseBuffDuration;
        }

        // Avoids double calling coroutine, but will still reset buff duration
        // if reset while buff is active
        if (!defenseIsBuffed)
        {
            StartCoroutine(DefenseBuff(player));
        }
    }

    private IEnumerator DefenseBuff(GameObject p)
    {
        // Maks coroutine as ongoing
        defenseIsBuffed = true;

        // Retrieve old defense to restore once buff ends
        int oldDefense = p.GetComponent<PlayerStats>().getDefense();

        // Assigns new defense
        p.GetComponent<PlayerStats>().setDefense(Mathf.FloorToInt(oldDefense * defenseBuffPercent));

        // Buff countdown, should allow for re-application with no issues, and cancelling of buffs
        while (defenseBuffTimer > 0)
        {
            Debug.Log("Defense buff has: " + defenseBuffTimer + " left.");
            yield return new WaitForSeconds(.1f);
            defenseBuffTimer -= .1f;
        }

        // Resets defense
        p.GetComponent<PlayerStats>().setDefense(oldDefense);

        // Marks coroutine as having ended
        defenseIsBuffed = false;
    }

    // Buffs own speed
    private void BlueGreenGreen()
    {
        // Sets buff duration to 10 seconds
        // Checks that current duration is not shorter than default,
        // due to extention effects from other combos
        if (speedBuffTimer < speedBuffDuration)
        {
            speedBuffTimer = speedBuffDuration;
        }

        // Avoids double calling coroutine, but will still reset buff duration
        // if reset while buff is active
        if (!speedIsBuffed)
        {
            StartCoroutine(SpeedBuff(player));
        }
    }

    private IEnumerator SpeedBuff(GameObject p)
    {
        // Maks coroutine as ongoing
        speedIsBuffed = true;

        // Retrieve old speed to restore once buff ends
        float oldSpeed = p.GetComponent<PlayerMovement>().getSpeed();

        // Assigns new speed
        p.GetComponent<PlayerMovement>().setSpeed(oldSpeed * speedBuffPercent);

        // Buff countdown, should allow for re-application with no issues, and cancelling of buffs
        while (speedBuffTimer > 0)
        {
            Debug.Log("Speed buff has: " + speedBuffTimer + " left.");
            yield return new WaitForSeconds(.1f);
            speedBuffTimer -= .1f;
        }

        // Resets speed
        p.GetComponent<PlayerMovement>().setSpeed(oldSpeed);

        // Marks coroutine as having ended
        speedIsBuffed = false;
    }

    // Large heal to self
    private void GreenGreenGreen()
    {
        // Make scale off something later??
        player.GetComponent<PlayerStats>().takeDamage(-30);
    }
}
