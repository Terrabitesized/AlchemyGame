using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class CrystalProjectile : MonoBehaviour
{
    [SerializeField] private AnimationCurve flySpeedCurve;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AnimationCurve spreadCurve;
    [SerializeField] private AnimationCurve heightOffSetCurve;
    [SerializeField] private float spreadMultiplier;
    [SerializeField] private float spreadDistance;
    [SerializeField] private float expectedTravelTime = 2f;
    [SerializeField] private Transform auraTransform;

    private Coroutine travelCoroutine;

    private Spell spell;
    private int CRYSTAL_COLOR = Shader.PropertyToID("CrystalColor");
    private bool target = false;

    public void Initialize(CombatCrystal crystal)
    {
        spell = crystal.crystalSpell;

        GetComponent<VisualEffect>().SetVector4(CRYSTAL_COLOR, crystal.color);
    }

    public void Fire()
    {
        // TODO CYRENE: Calculate target based on spell parameters
        transform.SetParent(null);
        target = true;
        travelCoroutine = StartCoroutine(FireCrystal());
    }

    private void ResetCrystal()
    {
        Debug.Log("AM I GETTING CALLED?");
        // NEEDS TO RESET THEIR POSITION???
        if(travelCoroutine != null )
           StopCoroutine(travelCoroutine);

        gameObject.SetActive(false);

        target = false;
    }

    private IEnumerator FireCrystal()
    {
        float travelTime = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(0f, 1f, 0f);

        while (Vector3.Distance(transform.position, targetPosition) > .1f)
        {
            if (!CombatManager.Instance.combatFlow)
            {
                yield return null;
                continue;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            travelTime += Time.deltaTime;

            yield return null;
        }

        spell.spellAbility.Target(CombatManager.Instance.GetTargetingManager(),
            CombatManager.Instance.GetPlayerDamagable());

        ResetCrystal();
    }
}
