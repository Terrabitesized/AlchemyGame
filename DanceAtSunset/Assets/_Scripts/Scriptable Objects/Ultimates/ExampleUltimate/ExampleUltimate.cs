using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Ultimates/Example Ultimate")]
public class ExampleUltimate : Ultimate
{
    public override IEnumerator PlayUltimate(CombatManager combatManager)
    {
        Debug.Log("Playing Example Ultimate");

        // Play unique animation here
        AnimatorClipInfo[] clipInfo = CutsceneAnimator.GetCurrentAnimatorClipInfo(0);

        // Wait until the attract screen has finished fading before allowing the trip to advance
        while (clipInfo[0].clip.name != "ExampleUltimate")
        {
            clipInfo = CutsceneAnimator.GetCurrentAnimatorClipInfo(0);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitUntil(() => CutsceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

        yield return new WaitForSeconds(1f);

        // Apply actual ultimate effect
        // ...
        //if (ultimateAbility != null)
        //    ultimateAbility.Target(combatManager.GetTargetingManager(), combatManager.GetPlayerDamagable());
    }
}