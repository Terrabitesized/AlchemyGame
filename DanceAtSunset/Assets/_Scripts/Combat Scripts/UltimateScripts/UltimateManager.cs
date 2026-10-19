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
        combatManager.combatFlow = false;
        // Disable all things
        yield return FadeWhiteScreen(true);
        CrystalAura.Instance?.ToggleCrystalVisibility(false);
        CombatCanvas.Instance?.ToggleCombatUIVisibility(false);
        CombatManager.Instance?.ClearAllSpawnedCrystals();
        CombatMovement.Instance?.TogglePlayerCameraRotatable(false);
        yield return FadeWhiteScreen(false);

        Debug.Log($"Playing ultimate: {ultimate.name}");

        // Execute ultimate
        yield return ultimate.PlayUltimate(combatManager);

        // Re-enable all things
        yield return FadeWhiteScreen(true);
        CrystalAura.Instance.ToggleCrystalVisibility(true);
        CombatCanvas.Instance?.ToggleCombatUIVisibility(true);
        CombatMovement.Instance?.TogglePlayerCameraRotatable(true);
        yield return FadeWhiteScreen(false);

        ultimate.ultimateAbility.Target(combatManager.GetTargetingManager(),
            combatManager.GetPlayerDamagable());

        combatManager.combatFlow = true;
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
