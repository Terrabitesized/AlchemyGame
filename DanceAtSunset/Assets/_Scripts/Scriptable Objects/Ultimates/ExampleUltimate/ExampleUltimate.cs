using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Ultimates/Example Ultimate")]
public class ExampleUltimate : Ultimate
{
    public override IEnumerator PlayUltimate(CombatManager combatManager)
    {
        Debug.Log("Playing Example Ultimate");

        // Play unique animation here

        yield return new WaitForSeconds(2f);

        // Apply actual ultimate effect
        // ...
        if (ultimateAbility != null)
            ultimateAbility.Target(combatManager.GetTargetingManager(), combatManager.GetPlayerDamagable());
    }
}