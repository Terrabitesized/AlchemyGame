using UnityEngine;

public class UltimateAttackExecuter : MonoBehaviour
{
    [SerializeField] private Ability[] ultimateAbilities;

    private IDamagable player;
    private TargetingManager targetingManager;

    private void OnEnable()
    {
        player = CombatManager.Instance?.GetPlayerDamagable();
        targetingManager = CombatManager.Instance?.GetTargetingManager();

        UltimateAnimationEventForwarder.OnUltimateAbilityProc += ExecuteAbility;
    }

    private void OnDisable()
    {
        UltimateAnimationEventForwarder.OnUltimateAbilityProc -= ExecuteAbility;
    }

    private void ExecuteAbility(int index)
    {
        if(index > ultimateAbilities.Length)
        {
            Debug.LogError("Ultimate Ability Index was out of bounds!");
            return;
        }

        ultimateAbilities[index].Target(targetingManager, player);
    }
}
