using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.VFX;

public class PotionManager : MonoBehaviour
{
    [SerializeReference] public Ability[] potionAbilities;
    [SerializeReference] public TargetingManager targetingManager;

    [Header("Inherited Variables")]
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> enemiesInCombat;
    private GameObject[] temp;

    [Header("Attack Buff Variables")]
    [SerializeField] private float attackBuffTimer = 0f;
    [SerializeField] private float attackBuffPercent = 1.3f;
    private bool attackIsBuffed = false;
    [SerializeField] private float attackBuffDuration = 30f;
    [SerializeField] private GameObject attackBuffVFX;

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

    private CombatManager cm;

    // Combat actions
    public static event Action<List<GameObject>> OnAttackBegin;
    public static event Action<List<GameObject>, List<bool>> OnAttackEnd;

    // DAMAGE FORMULA
    // (ATK ^ 1.2 / DEF + 20) * Level Mod * Base POW

    // Level Mod: 1 + ((PLV - ELV) * .05)
    // This grants +5% dmg for 1 level higher player
    private void Start()
    {
        cm = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();
    }

    public void SetupPM(GameObject p, List<GameObject> e)
    {
        player = p;
        enemiesInCombat = e;
        targetingManager = player.GetComponent<TargetingManager>();
    }

    public int CalculateDamage(GameObject player, GameObject enemy, int basePower)
    {
        float levelMod = ((player.GetComponent<PlayerStats>().getLevel() - enemy.GetComponent<EnemyStats>().getLevel()) * .05f) + 1;
        float playerAttack = player.GetComponent<PlayerStats>().getAttack();
        float enemyDefense = enemy.GetComponent<EnemyStats>().getDefense();

        Debug.Log("Level Mod: " + levelMod);
        Debug.Log("Player Attack: " + playerAttack);
        Debug.Log("Enemy Def: " + enemyDefense);


        return Mathf.RoundToInt((Mathf.Pow(playerAttack, 1.2f) / (enemyDefense + 20f)) * basePower * levelMod) + 1;
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
        //List<GameObject> attackedEnemies = new List<GameObject>();
        //List<bool> enemiesAlive = new List<bool>();

        //// Targets a random enemy on field
        //int enemyTargetIndex = UnityEngine.Random.Range(0, enemiesInCombat.Count);

        //// Deals damage to targeted enemy
        //int damage = CalculateDamage(player, enemiesInCombat[enemyTargetIndex], 10);

        //// Deals damage, and returns true if the enemy is still alive, false if they have perished
        //enemiesAlive.Add(enemiesInCombat[enemyTargetIndex].GetComponent<IDamagable>().takeDamage(damage));

        //// Adds a track of all enemies hit
        //attackedEnemies.Add(enemiesInCombat[enemyTargetIndex]);
        //cm.increaseDamageDealt(damage);

        //OnAttackEnd?.Invoke(attackedEnemies, enemiesAlive);

        //cm.ProcessEnemyDeaths();

        // Targets a random enemy on field
        int enemyTargetIndex = UnityEngine.Random.Range(0, enemiesInCombat.Count);

        // Deals damage to targeted enemy
        int damage = CalculateDamage(player, enemiesInCombat[enemyTargetIndex], 10);

        // Deals damage, and returns true if the enemy is still alive, false if they have perished
        //potionAbilities[0].Execute(enemiesInCombat[enemyTargetIndex].GetComponent<IDamagable>());
        potionAbilities[0].Target(targetingManager);
    }

    // Deals small DMG to all enemies
    private void RedRedBlue()
    {
        // Targets all enemies on field
        temp = enemiesInCombat.ToArray();

        List<GameObject> attackedEnemies = new List<GameObject>();
        List<bool> enemiesAlive = new List<bool>();

        foreach (GameObject enemy in temp)
        {
            int damage = CalculateDamage(player, enemy, 4);

            // Deals damage, and returns true if the enemy is still alive, false if they have perished
            enemiesAlive.Add(enemy.GetComponent<EnemyStats>().takeDamage(damage));

            // Adds a track of all enemies hit
            attackedEnemies.Add(enemy);

            cm.increaseDamageDealt(damage);
        }

        OnAttackEnd?.Invoke(attackedEnemies, enemiesAlive);

        cm.ProcessEnemyDeaths();
    }

    // Deals bounce DMG to random enemy(s)
    private void RedRedGreen()
    {
        temp = enemiesInCombat.ToArray();
        StartCoroutine(BounceDamage(player, temp));
    }

    private IEnumerator BounceDamage(GameObject p, GameObject[] enemies)
    {
        for (int i = 0; i < 3; i++)
        {
            // Checks that the bounce attack hasn't killed all enemies to early
            if(!cm.isBattleOver)
            {
                // Targets a random enemy on field
                int enemyTargetIndex = UnityEngine.Random.Range(0, temp.Length);

                // Ensures that if an enemy dies mid-bounce attack, it will pick a new valid target
                while (temp[enemyTargetIndex] == null)
                {
                    enemyTargetIndex = UnityEngine.Random.Range(0, temp.Length);
                }

                // Deals damage to targeted enemy
                int damage = CalculateDamage(player, temp[enemyTargetIndex], 3);
                temp[enemyTargetIndex].GetComponent<EnemyStats>().takeDamage(damage);
                cm.increaseDamageDealt(damage);
                yield return new WaitForSeconds(.1f);
            } 
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

        GameObject buff = Instantiate(attackBuffVFX, player.transform.position, Quaternion.identity);
        buff.transform.parent = player.transform;
        buff.GetComponent<VisualEffect>().SetFloat("Lifetime", attackBuffDuration);
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
        player.GetComponent<IDamagable>().takeDamage(-15);
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
        player.GetComponent<IDamagable>().takeDamage(-30);
    }
}
