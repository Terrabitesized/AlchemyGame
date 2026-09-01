using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyAttackPattern
{
    public string AttackName;

    public float AttackCastTime;
    public float AdditionalAttackCooldown;

    public GameObject WarningPrefab;
    public GameObject AttackPrefab;
    public float WarningDuration;
    public float AttackDuration;

    protected List<GameObject> enemyAttackWarningPool = new List<GameObject>();
    protected List<GameObject> enemyAttackPool = new List<GameObject>();

    protected EnemyAbility abilty;

    public abstract void Start(EnemyAbility ability);
    public virtual void Update() { }
    public virtual void Cancel() { }

    public GameObject GetPooledWarning()
    {
        for (int i = 0; i < enemyAttackWarningPool.Count; i++)
        {
            if (!enemyAttackWarningPool[i].activeInHierarchy)
            {
                return enemyAttackWarningPool[i];
            }
        }

        GameObject temp = UnityEngine.Object.Instantiate(WarningPrefab);
        enemyAttackWarningPool.Add(temp);
        return temp;
    }
    public GameObject GetPooledAttack()
    {
        for (int i = 0; i < enemyAttackPool.Count; i++)
        {
            if (!enemyAttackPool[i].activeInHierarchy)
            {
                return enemyAttackPool[i];
            }
        }

        GameObject temp = UnityEngine.Object.Instantiate(AttackPrefab);
        enemyAttackPool.Add(temp);
        return temp;
    }
}

[Serializable]
public class RandomDamageZoneTargeting : EnemyAttackPattern
{
    public float AttackPrefabScale = 1f;
    public int DamageZoneCount = 5;
    private Vector3[] zonePositions;

    public override void Start(EnemyAbility ability)
    {
        this.abilty = ability;
        zonePositions = new Vector3[DamageZoneCount];

        for (int i = 0; i < DamageZoneCount; i++)
        {
            float x_Pos = UnityEngine.Random.Range(-18f, 18f);
            float z_Pos = UnityEngine.Random.Range(-18f, 18f);

            GameObject warning = GetPooledWarning();

            Vector3 temp = new Vector3(x_Pos, 0f, z_Pos);
            zonePositions[i] = temp;
            warning.transform.position = temp;
            warning.transform.localScale = Vector3.one * AttackPrefabScale;

            warning.GetComponent<EnemyAttackHitbox>()?.Init(null, WarningDuration);

            warning.SetActive(true);
        }

        CoroutineRunner.Instance?.StartCoroutine((SpawnDamageZone()));
    }

    public IEnumerator SpawnDamageZone()
    {
        // Allow damage warnings to live their intended lifetimes
        yield return new WaitForSeconds(WarningDuration);

        for (int i = 0; i < DamageZoneCount; i++)
        {
            GameObject attack = GetPooledAttack();

            attack.transform.position = new Vector3(zonePositions[i].x, 0f, zonePositions[i].z);
            attack.GetComponent<EnemyAttackHitbox>()?.Init(abilty, WarningDuration);
            attack.transform.localScale = Vector3.one * AttackPrefabScale;

            attack.SetActive(true);
        }
    }
}

[Serializable]
public class TargetedDamageZoneTargeting : EnemyAttackPattern
{
    public float AttackPrefabScale = 1f;
    public int DamageZoneCount = 5;
    public float DamageZoneSpawnDelay = 1f;

    private GameObject player;

    public override void Start(EnemyAbility ability)
    {
        this.abilty = ability;
        player = CombatManager.Instance?.GetPlayerGameObject();

        CoroutineRunner.Instance?.StartCoroutine((StartSpawningZones()));
    }

    public IEnumerator StartSpawningZones()
    {
        for (int i = 0; i < DamageZoneCount; i++)
        {
            GameObject warning = GetPooledWarning();

            Vector3 temp = new Vector3(player.transform.position.x, 0f, 
                player.transform.position.z);

            warning.transform.position = temp;
            warning.transform.localScale = Vector3.one * AttackPrefabScale;

            warning.GetComponent<EnemyAttackHitbox>()?.Init(null, WarningDuration);

            warning.SetActive(true);

            CoroutineRunner.Instance?.StartCoroutine((SpawnDamageZone(temp)));

            yield return new WaitForSeconds(DamageZoneSpawnDelay);
        }
    }

    public IEnumerator SpawnDamageZone(Vector3 spawnPosition)
    {
        // Allow damage warnings to live their intended lifetimes
        yield return new WaitForSeconds(WarningDuration);

        GameObject attack = GetPooledAttack();

        attack.transform.position = spawnPosition;
        attack.GetComponent<EnemyAttackHitbox>()?.Init(abilty, WarningDuration);
        attack.transform.localScale = Vector3.one * AttackPrefabScale;

        attack.SetActive(true);
    }
}

[Serializable]
public class ShockwaveTargeting : EnemyAttackPattern {

    [Header("Wave Shape")]
    public float AttackPrefabScale = 1f;

    // How many hitboxes lengthwise
    public int length = 8;

    // How many hitboxes widthwise (starting)
    public int width = 1;

    // Distance between each row
    public float RowSpacing = 1f;

    // Does wave get wider?
    public bool Expanding = false;
    // How much with each step?
    public int WidthStep = 1;

    // Max width achievable
    public int MaxWidth = 5;

    [Header("Wave Timing")]
    public float DamageZoneSpawnDelay = 0.15f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 dir;
    private Vector3 perpDir;

    private GameObject player;

    public override void Start(EnemyAbility ability)
    {
        this.abilty = ability;

        // Get player postition once
        startPos = ability.ownerTransform.position;

        player = CombatManager.Instance?.GetPlayerGameObject();

        if (player == null) {
            return;
        }

        targetPos = player.transform.position;

        startPos.y = 0f; 
        targetPos.y = 0f;

        // Calculate direction and perpendicular vector
        dir = (targetPos - startPos).normalized;

        perpDir = new Vector3(-dir.z, 0f, dir.x);

        CoroutineRunner.Instance?.StartCoroutine(SpawnWave());
        
    }

    private IEnumerator SpawnWave()
    {

        for (int i = 0; i < length; i++)
        {
            // Calculate the position for this row
            Vector3 rowPosition = startPos + dir * (RowSpacing * (i + 1));

            // Calculate the width for this row
            int currentWidth = width;

            // Increase width if expanding
            if (Expanding)
            {
                currentWidth = Mathf.Min(width + (i * WidthStep), MaxWidth);
            }

            // Spawn warnings for each row individually
            SpawnWarningRow(rowPosition, currentWidth);

            // Wait for the next row to spawn
            yield return new WaitForSeconds(DamageZoneSpawnDelay);
        }

    }

    private void SpawnWarningRow(Vector3 rowCenter, int width)
    {
        // Spawn warnings
        for (int x = 0; x < width; x++)
        {
            float offset = (x - (width - 1) / 2f) * AttackPrefabScale;

            Vector3 spawnPosition = rowCenter + perpDir * offset;

            GameObject warning = GetPooledWarning();

            warning.transform.position = spawnPosition;
            warning.transform.localScale = Vector3.one * AttackPrefabScale;

            warning.GetComponent<EnemyAttackHitbox>()?.Init(null, WarningDuration);

            warning.SetActive(true);

            // Create actual attack
            CoroutineRunner.Instance?.StartCoroutine(SpawnDamageZone(spawnPosition));
        }
    }

    private IEnumerator SpawnDamageZone(Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(WarningDuration);

        GameObject attack = GetPooledAttack();

        attack.transform.position = spawnPosition;
        attack.GetComponent<EnemyAttackHitbox>()?.Init(abilty, WarningDuration);
        attack.transform.localScale = Vector3.one * AttackPrefabScale;

        attack.SetActive(true);
    }

}