using System;
using System.Collections;
using UnityEngine;

public class UltimateManager : MonoBehaviour
{
    public static UltimateManager Instance;

    public static Action OnUltimateStart;
    public static Action OnUltimateEnd;

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

        OnUltimateStart?.Invoke();

        CrystalAura.Instance?.ToggleCrystalVisibility(false);
        CombatCanvas.Instance?.ToggleCombatUIVisibility(false);
        CombatManager.Instance?.ToggleArenaBarrierVisibility(false);
        CombatMovement.Instance?.TogglePlayerCameraRotatable(false);
        CombatPopupManager.Instance?.HideAllDamagePopups();


        if(ultimate.HidePlayer)
            CombatManager.Instance?.TogglePlayerVisibility(false);

        if (ultimate.HideMap)
            CombatManager.Instance?.ToggleMapVisibility(false);

        // Enable ultimate resources
        CombatManager.Instance.ToggleCutsceneResources(true);

        yield return FadeWhiteScreen(false);

        Debug.Log($"Playing ultimate: {ultimate.name}");

        // Execute ultimate
        yield return ultimate.PlayUltimate(combatManager);

        // Re-enable all things
        yield return FadeWhiteScreen(true);

        OnUltimateEnd?.Invoke();

        CrystalAura.Instance.ToggleCrystalVisibility(true);
        CombatCanvas.Instance?.ToggleCombatUIVisibility(true);
        CombatManager.Instance?.ToggleArenaBarrierVisibility(true);
        CombatMovement.Instance?.TogglePlayerCameraRotatable(true);

        // Enable ultimate resources
        CombatManager.Instance.ToggleCutsceneResources(false);

        if (ultimate.HidePlayer)
            CombatManager.Instance?.TogglePlayerVisibility(true);

        if (ultimate.HideMap)
            CombatManager.Instance?.ToggleMapVisibility(true);
        yield return FadeWhiteScreen(false);

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
