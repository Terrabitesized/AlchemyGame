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

         // NEEDS TO RESET THEIR POSITION???
        target = false;
    }

    private void Update()
    {
        if (!target || !CombatManager.Instance.combatFlow)
            return;
    }

    private IEnumerator FireCrystal()
    {
        bool shouldTravel = true;
        float travelTime = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(0f, 1f, 0f);

        while (shouldTravel)
        {
            if (!CombatManager.Instance.combatFlow)
            {
                yield return null;
                continue;
            }

            // Check if we are close enough to the position
            if(Vector3.Distance(transform.position, targetPosition) < .1f)
            {
                shouldTravel = false;
                ResetCrystal();
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            travelTime += Time.deltaTime;

            yield return null;
        }
    }
}
