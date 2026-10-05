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

        yield return FadeToWhite();

        if (ultimate == null)
        {
            Debug.LogError("UltimateManager: ultimate is NULL!");
            yield break;
        }

        if (combatManager == null)
        {
            Debug.LogError("UltimateManager: combatManager is NULL!");
            yield break;
        }

        Debug.Log($"Playing ultimate: {ultimate.name}");

        yield return ultimate.PlayUltimate(combatManager);

        yield return FadeFromWhite();

        combatManager.combatFlow = true;
    }

    private IEnumerator FadeToWhite(float duration = 0.5f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            whiteScreen.alpha = Mathf.Clamp01(elapsed / duration);

            yield return null;
        }

        whiteScreen.alpha = 1f;
    }

    private IEnumerator FadeFromWhite(float duration = 0.5f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            whiteScreen.alpha = 1f - Mathf.Clamp01(elapsed / duration);

            yield return null;
        }

        whiteScreen.alpha = 0f;
    }
}
