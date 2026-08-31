using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyAttackPattern
{
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
