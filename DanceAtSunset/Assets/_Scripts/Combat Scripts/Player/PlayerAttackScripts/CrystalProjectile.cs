using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class CrystalProjectile : MonoBehaviour
{
    [SerializeField] private AnimationCurve moveSpeedCurve;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private AnimationCurve spreadCurve;
    [SerializeField] private float spreadMultiplier;
    [SerializeField] private float spreadDistance = .5f;
    [SerializeField] private float expectedTravelTime = 2f;

    private Coroutine travelCoroutine;

    private Spell spell;
    private int CRYSTAL_COLOR = Shader.PropertyToID("CrystalColor");

    public void Initialize(CombatCrystal crystal, int spreadMultiplier)
    {
        spell = crystal.crystalSpell;
        this.spreadMultiplier = spreadMultiplier;

        GetComponent<VisualEffect>().SetVector4(CRYSTAL_COLOR, crystal.color);
    }

    public void Fire()
    {
        // TODO CYRENE: Calculate target based on spell parameters
        transform.SetParent(null);
        travelCoroutine = StartCoroutine(FireCrystal());
    }

    private void ResetCrystal()
    {
        Debug.Log("AM I GETTING CALLED?");
        // NEEDS TO RESET THEIR POSITION???
        if(travelCoroutine != null )
           StopCoroutine(travelCoroutine);

        gameObject.SetActive(false);
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
                * spreadMultiplier
                * spreadMultiplier;

            transform.position =
                startPosition + spreadDirectionVector * spreadAmount;

            travelTime += Time.deltaTime;

            yield return null;
        }

        spell.spellAbility.Target(CombatManager.Instance.GetTargetingManager(),
            CombatManager.Instance.GetPlayerDamagable());

        ResetCrystal();
    }
}
