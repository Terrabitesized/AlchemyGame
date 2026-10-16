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

    public void Initialize(CombatCrystal crystal)
    {
        spell = crystal.crystalSpell;

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
            float speedMult = moveSpeedCurve.Evaluate(travelTime / expectedTravelTime);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * speedMult * Time.deltaTime);
            travelTime += Time.deltaTime;

            yield return null;
        }

        spell.spellAbility.Target(CombatManager.Instance.GetTargetingManager(),
            CombatManager.Instance.GetPlayerDamagable());

        ResetCrystal();
    }
}
