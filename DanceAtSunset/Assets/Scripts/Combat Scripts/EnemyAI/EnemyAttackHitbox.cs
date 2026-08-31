using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private EnemyAbility ability;
    private float duration;

    public void Init(EnemyAbility ability, float duration)
    {
        this.ability = ability; // if Ability == null, this is a warning, and not an actual attack
        this.duration = duration;

        Invoke("DisableSelf", duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ability == null)
            return;

        if (other.CompareTag("Player"))
        {
            IDamagable target = other.GetComponent<IDamagable>();
            if (target != null)
            {
                ability.Execute(target);
            }
        }
    }

    private void DisableSelf()
    {
        this.gameObject.SetActive(false);
    }
}
