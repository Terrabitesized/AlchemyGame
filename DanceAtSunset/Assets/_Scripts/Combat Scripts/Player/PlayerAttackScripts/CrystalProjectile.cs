using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class CrystalProjectile : MonoBehaviour
{
    [SerializeField] private AnimationCurve flySpeedCurve;
    [SerializeField] private AnimationCurve spreadCurve;
    [SerializeField] private AnimationCurve heightOffSetCurve;
    [SerializeField] private float spreadMultiplier;
    [SerializeField] private float spreadDistance;
    private Transform auraTransform;

    private Spell spell;
    private int CRYSTAL_COLOR = Shader.PropertyToID("CrystalColor");
    private bool target = false;

    public void Initialize(CombatCrystal crystal)
    {
        spell = crystal.crystalSpell;
        GetComponent<VisualEffect>().SetVector4(CRYSTAL_COLOR, crystal.color);
        gameObject.SetActive(true);
    }

    public void Fire()
    {
        // TODO CYRENE: Calculate target based on spell parameters
        transform.SetParent(null);
        target = true;
        StartCoroutine(FireCrystal());
    }

    private void ResetCrystal()
    {
        transform.SetParent(auraTransform);
        target = false;
    }

    private void Update()
    {
        if (!target || !CombatManager.Instance.combatFlow)
            return;
    }

    private IEnumerator FireCrystal()
    {
        float travelTime = 0f;
        float duration = 5f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(0f, 1f, 0f);

        Vector3 direction = targetPosition - startPosition;
        direction.y = 0f;
        direction.Normalize();

        Vector3 sideDirection = Vector3.Cross(
            Vector3.up,
            direction
        ).normalized;

        while (travelTime < duration)
        {
            if (!CombatManager.Instance.combatFlow)
            {
                yield return null;
                continue;
            }

            travelTime += Time.deltaTime;

            float t = Mathf.Clamp01(travelTime / duration);

            // Base position along the path.
            float progress = Mathf.Clamp01(flySpeedCurve.Evaluate(t));

            Vector3 basePosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                progress
            );

            // Calculate offsets independently each frame.
            float spread = spreadCurve.Evaluate(t)
                * spreadDistance
                * spreadMultiplier;

            float height = heightOffSetCurve.Evaluate(t);

            transform.position = basePosition
                + sideDirection * spread
                + Vector3.up * height;

            yield return null;
        }

        transform.position = targetPosition;
        //ResetCrystal();
    }
}
