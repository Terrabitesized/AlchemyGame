using System;
using System.Collections.Generic;
using UnityEngine;

public class UltimateAnimationEventForwarder : MonoBehaviour
{
    public static event Action<int> OnUltimateAbilityProc;
    public static Action OnUltimateFinalHit;

    public void ForwardUltimateAbility(int abilityIndex = 0)
    {
        OnUltimateAbilityProc?.Invoke(abilityIndex);
    }

    public void ForwardUltimateFinalHit()
    {
        OnUltimateFinalHit?.Invoke();
    }
}
