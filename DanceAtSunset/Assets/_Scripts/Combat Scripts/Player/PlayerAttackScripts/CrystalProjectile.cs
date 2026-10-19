using System.Collections;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class CrystalProjectile : MonoBehaviour
{
    [SerializeField] private GameObject subVFX;
    [SerializeField] private TrailRenderer trail;

    [Header("Animation Parameters")]
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private float scaleUpTime = .5f;

    [SerializeField] private AnimationCurve moveSpeedCurve;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private AnimationCurve spreadCurve;
    [SerializeField] private float spreadMultiplier;
    [SerializeField] private float spreadDistance = .5f;
    [SerializeField] private float expectedTravelTime = 2f;

    private Coroutine travelCoroutine;
    private float initialScale;

    private Spell spell;
    private int CRYSTAL_COLOR = Shader.PropertyToID("CrystalColor");

    private void Awake()
    {
        initialScale = transform.localScale.x;
        subVFX.SetActive(false);
        trail.enabled = false;
    }

    public void Initialize(CombatCrystal crystal, int spreadMultiplier)
    {
        spell = crystal.crystalSpell;
        this.spreadMultiplier = spreadMultiplier;

        GetComponent<VisualEffect>().SetVector4(CRYSTAL_COLOR, crystal.color);
        subVFX.GetComponent<VisualEffect>().SetVector4(CRYSTAL_COLOR, crystal.color);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(crystal.color, 0.0f), new GradientColorKey(crystal.color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(.65f, 0.0f), new GradientAlphaKey(0f, 1.0f) }
            );

        trail.colorGradient = gradient;

        StartCoroutine(ScaleUp());
    }

    private IEnumerator ScaleUp()
    {
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / scaleUpTime;

            transform.localScale = Vector3.one * Mathf.Lerp(0f, initialScale, scaleCurve.Evaluate(progress));
        }
    }

    public void Fire()
    {
        // TODO CYRENE: Calculate target based on spell parameters
        transform.SetParent(null);
        subVFX.SetActive(true);
        trail.enabled = true;

        travelCoroutine = StartCoroutine(FireCrystal());
    }

    private void ResetCrystal()
    {
        Debug.Log("AM I GETTING CALLED?");
        // NEEDS TO RESET THEIR POSITION???
        if(travelCoroutine != null )
           StopCoroutine(travelCoroutine);

        subVFX.SetActive(false);
        gameObject.SetActive(false);
        trail.enabled = false;
    }

    private IEnumerator FireCrystal()
    {
        float travelTime = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(0f, 1f, 0f);

        // TEMP

        Vector3 travelDirection =
            (targetPosition - startPosition).normalized;

        // A direction perpendicular to the travel path.
        Vector3 spreadDirectionVector =
            Vector3.Cross(travelDirection, Vector3.up).normalized;

        // Handle a nearly vertical travel direction.
        if (spreadDirectionVector.sqrMagnitude < 0.001f)
            spreadDirectionVector = Vector3.right;

        // TEMP


        // Travel towards enemy
        while (Vector3.Distance(transform.position, targetPosition) > .1f)
        {
            // If combat is paused, don't move
            if (!CombatManager.Instance.combatFlow)
            {
                yield return null;
                continue;
            }

            // Incredment speed over the first few seconds of travel.
            //float speedMult = moveSpeedCurve.Evaluate(travelTime / expectedTravelTime);

            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * speedMult * Time.deltaTime);

            float progress = Mathf.Clamp01(
                travelTime / expectedTravelTime
            );

            float speedMult =
                moveSpeedCurve.Evaluate(progress);

            // Move the underlying trajectory toward the target.
            startPosition = Vector3.MoveTowards(
                startPosition,
                targetPosition,
                moveSpeed * speedMult * Time.deltaTime
            );

            // Evaluate the outward spread, then bring it back to zero.
            float spreadAmount =
                spreadCurve.Evaluate(progress)
                * spreadDistance
                * spreadMultiplier;

            float heightOffset = 
                spreadCurve.Evaluate(progress)
                * spreadDistance
                * (-Mathf.Abs(spreadMultiplier) + 1);

            transform.position =
                startPosition + spreadDirectionVector * spreadAmount
                + Vector3.up * heightOffset;

            travelTime += Time.deltaTime;

            yield return null;
        }

        spell.spellAbility.Target(CombatManager.Instance.GetTargetingManager(),
            CombatManager.Instance.GetPlayerDamagable());

        ResetCrystal();
    }
}
