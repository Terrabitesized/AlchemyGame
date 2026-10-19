using System.Collections;
using UnityEngine;

public class UltimateManager : MonoBehaviour
{
    public static UltimateManager Instance;

    [SerializeField] private CanvasGroup whiteScreen;
    [SerializeField] private Ultimate currentUltimate;

    private void OnEnable()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDisable()
    {
        Instance = null;
    }

    public IEnumerator PlayUltimate(Ultimate ultimate, CombatManager combatManager)
    {
        combatManager.CombatFlow = false;
        // Disable all things
        yield return FadeWhiteScreen(true);
        CrystalAura.Instance?.ToggleCrystalVisibility(false);
        CombatCanvas.Instance?.ToggleCombatUIVisibility(false);
        CombatManager.Instance?.ClearAllSpawnedCrystals();
        CombatManager.Instance?.ToggleArenaBarrierVisibility(false);
        CombatMovement.Instance?.TogglePlayerCameraRotatable(false);


        if(ultimate.HideMap)
            CombatManager.Instance?.ToggleMapVisibility(false);

        yield return FadeWhiteScreen(false);

        Debug.Log($"Playing ultimate: {ultimate.name}");

        // Execute ultimate
        yield return ultimate.PlayUltimate(combatManager);

        // Re-enable all things
        yield return FadeWhiteScreen(true);
        CrystalAura.Instance.ToggleCrystalVisibility(true);
        CombatCanvas.Instance?.ToggleCombatUIVisibility(true);
        CombatManager.Instance?.ToggleArenaBarrierVisibility(true);
        CombatMovement.Instance?.TogglePlayerCameraRotatable(true);

        if(ultimate.HideMap)
            CombatManager.Instance?.ToggleMapVisibility(true);
        yield return FadeWhiteScreen(false);

        ultimate.UltimateAbility.Target(combatManager.GetTargetingManager(),
            combatManager.GetPlayerDamagable());

        combatManager.CombatFlow = true;
    }

    private IEnumerator FadeWhiteScreen(bool visible, float duration = 0.5f)
    {
        float start = whiteScreen.alpha;
        float target = visible ? 1f : 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            whiteScreen.alpha = Mathf.Lerp(start, target, elapsed / duration);

            yield return null;
        }

        whiteScreen.alpha = target;
    }
}
