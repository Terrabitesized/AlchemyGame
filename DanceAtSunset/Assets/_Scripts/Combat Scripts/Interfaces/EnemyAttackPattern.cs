using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public abstract class EnemyAttackPattern
{
    [Header("Attack Parameters")]
    public string AttackName;

    public float AttackCastTime;
    public bool CanBeConsecutive = true;

    [Header("Attack Prefabs & Durations")]
    public GameObject WarningPrefab;
    public GameObject AttackPrefab;
    public float WarningDuration;
    public float AttackDuration;

    protected List<GameObject> enemyAttackWarningPool = new List<GameObject>();
    protected List<GameObject> enemyAttackPool = new List<GameObject>();

    protected EnemyAbility abilty;
    protected IDamagable attacker;

    public abstract void Start(EnemyAbility ability, IDamagable attacker);
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

    public override void Start(EnemyAbility ability, IDamagable attacker)
    {
        this.abilty = ability;
        this.attacker = attacker;
        zonePositions = new Vector3[DamageZoneCount];

        float arenaSize = CombatManager.Instance != null ? 
            CombatManager.Instance.arenaSize : 10f;

        for (int i = 0; i < DamageZoneCount; i++)
        {
            float x_Pos = UnityEngine.Random.Range(-arenaSize, arenaSize);
            float z_Pos = UnityEngine.Random.Range(-arenaSize, arenaSize);

            while (Vector2.Distance(new Vector2(x_Pos, z_Pos), new Vector2(0.0f, 0.0f)) > arenaSize)
            {
                x_Pos = UnityEngine.Random.Range(-arenaSize, arenaSize);
                z_Pos = UnityEngine.Random.Range(-arenaSize, arenaSize);
            }

            GameObject warning = GetPooledWarning();

            Vector3 temp = new Vector3(x_Pos, 0f, z_Pos);
            zonePositions[i] = temp;
            warning.transform.position = temp;
            warning.transform.localScale = Vector3.one * AttackPrefabScale;

            warning.GetComponent<EnemyAttackHitbox>()?.Init(null, attacker, WarningDuration);

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
            attack.GetComponent<EnemyAttackHitbox>()?.Init(abilty, attacker, WarningDuration);
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

    public override void Start(EnemyAbility ability, IDamagable attacker)
    {
        this.abilty = ability;
        this.attacker = attacker;
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

            warning.GetComponent<EnemyAttackHitbox>()?.Init(null, attacker, WarningDuration);

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
        attack.GetComponent<EnemyAttackHitbox>()?.Init(abilty, attacker, WarningDuration);
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

    public override void Start(EnemyAbility ability, IDamagable attacker)
    {
        this.abilty = ability;
        this.attacker = attacker;

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

            warning.GetComponent<EnemyAttackHitbox>()?.Init(null, attacker, WarningDuration);

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
        attack.GetComponent<EnemyAttackHitbox>()?.Init(abilty, attacker, WarningDuration);
        attack.transform.localScale = Vector3.one * AttackPrefabScale;

        attack.SetActive(true);
    }
}

[Serializable]
public class RotatingLineTargeting : EnemyAttackPattern
{

    [Header("Line Shape")]
    // Line length
    public float lineLength = 10f;
    // How wide
    public float lineWidth = 1f;
    // Number of lines
    public int lineCount = 1;

    [Header("Rotation")]
    // How may degrees to rotate per second
    public float rotationSpeed = 90f;
    // Rotation direction
    public bool clockwise = true;

    [Header("Timing")]
    public float DamageZoneSpawnDelay = 0.05f;

    [Header("Visuals")]
    public float AttackPrefabScale = 1f;

    private Vector3 centerPosition;
    private float currentAngle;
    private bool attacking = true; 
    
    private List<GameObject> activeWarnings = new List<GameObject>(); 
    private List<GameObject> activeAttacks = new List<GameObject>();

    public override void Start(EnemyAbility ability, IDamagable attacker)
    {
        this.abilty = ability;
        this.attacker = attacker;

        attacking = true;

        // Center of rotation
        centerPosition = ability.ownerTransform.position;
        centerPosition.y = 0f;

        // Get a random angle
        currentAngle = UnityEngine.Random.Range(0f, 360f);

        SpawnWarnings();

        CoroutineRunner.Instance?.StartCoroutine(RotateAttack());
    }

    private void SpawnWarnings()
    {
        activeWarnings.Clear();

        float angleStep = 360f / lineCount; for (int i = 0; i < lineCount; i++)
        {
            float angle = currentAngle + (i * angleStep);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
            GameObject warning = GetPooledWarning();
            warning.transform.localScale = new Vector3(lineWidth * AttackPrefabScale, AttackPrefabScale, lineLength * AttackPrefabScale);

            UpdateLineObject(warning, direction);
            warning.GetComponent<EnemyAttackHitbox>()?.Init(null, attacker, WarningDuration);
            warning.SetActive(true); activeWarnings.Add(warning);
        }
    }
    private IEnumerator RotateAttack()
    {
        yield return new WaitForSeconds(WarningDuration);

        if (!attacking) 
            yield break;

        activeAttacks.Clear();
        for (int i = 0; i < lineCount; i++)
        {
            GameObject attack = GetPooledAttack();
            UpdateLineObject(attack, GetLineDirection(currentAngle + (i * (360f / lineCount))));
            attack.transform.localScale = new Vector3(lineWidth * AttackPrefabScale, AttackPrefabScale, lineLength * AttackPrefabScale);
            attack.GetComponent<EnemyAttackHitbox>()?.Init(abilty, attacker, AttackDuration);
            attack.SetActive(true);
            activeAttacks.Add(attack);
        }

        float elapsedTime = 0f;

        while (attacking && elapsedTime < AttackDuration)
        {
            float dir = clockwise ? -1f : 1f;

            currentAngle += rotationSpeed * dir * Time.deltaTime;

            UpdateLinePositions();

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        attacking = false;
    }
    
    private void UpdateLinePositions()
    {
        if (lineCount <= 0) 
            return;

        float angleStep = 360f / lineCount;

        for (int i = 0; i < lineCount; i++)
        {
            float angle = currentAngle + (i * angleStep);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));

            // Rotate warning
            if ( (i < activeWarnings.Count && activeWarnings[i] != null) )
            {
                UpdateLineObject(activeWarnings[i], direction);
            }

            // Rotate attack
            if (i < activeAttacks.Count && activeAttacks[i] != null)
            {
                UpdateLineObject(activeAttacks[i], direction);
            }
        }
    }

    private Vector3 GetLineDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
    }

    private void UpdateLineObject(GameObject lineObject, Vector3 direction)
    {
        lineObject.transform.position = centerPosition + direction * (lineLength * AttackPrefabScale * 0.5f); 
        lineObject.transform.rotation = Quaternion.LookRotation(direction);
    }

    public override void Cancel()
    {
        attacking = false;
        foreach (var warning in activeWarnings)
        {
            if (warning != null)
                warning.SetActive(false);
        }
        foreach (var attack in activeAttacks)
        {
            if (attack != null)
                attack.SetActive(false);
        }
        activeWarnings.Clear();
        activeAttacks.Clear();
    }
}