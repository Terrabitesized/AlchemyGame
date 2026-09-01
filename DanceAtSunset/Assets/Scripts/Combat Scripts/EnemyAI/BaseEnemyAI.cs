using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class BaseEnemyAI : MonoBehaviour
{
    public List<EnemyAbility> EnemyAbilities;
    [SerializeField] private float attackCooldown = 5f;

    private void Start()
    {
        StartCoroutine(SelectAttack());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator SelectAttack()
    {
        while(true)
        {
            yield return new WaitForSeconds(attackCooldown);

            int attackChoice = Random.Range(0, EnemyAbilities.Count);

            EnemyAbilities[attackChoice].Target();
        }
    }
}
