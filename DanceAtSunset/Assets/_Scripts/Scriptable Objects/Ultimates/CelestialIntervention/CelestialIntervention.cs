using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Combat/Ultimates/CelestialIntervention")]
public class CelestialIntervention : Ultimate
{
    private bool ultimateComplete = false;

    public override IEnumerator PlayUltimate(CombatManager combatManager)
    {
        Debug.Log("Playing Celestial Intervention");

        //// Play unique animation here
        //AnimatorClipInfo[] clipInfo = CutsceneAnimator.GetCurrentAnimatorClipInfo(0);

        //// Wait until the attract screen has finished fading before allowing the trip to advance
        //while (clipInfo[0].clip.name != "CelestialIntervention")
        //{
        //    clipInfo = CutsceneAnimator.GetCurrentAnimatorClipInfo(0);
        //    yield return new WaitForEndOfFrame();
        //}

        //yield return new WaitUntil(() => CutsceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        //yield return new WaitUntil(() => ultimateComplete);

        CombatManager.Instance.InputHandler.PlayerInput.UI.Continue.performed += ProcessInput;
        yield return new WaitForSeconds(5f);
        CombatManager.Instance.InputHandler.PlayerInput.UI.Continue.performed -= ProcessInput;
    }

    private void ProcessInput(InputAction.CallbackContext c)
    {
        Debug.Log("Clicked during ultimate!!");
    }
}