using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageCrystal : MonoBehaviour, ICombatSpawnable
{
    public static Action<CombatCrystal> OnCrystalCollected;
    [SerializeField] private GameObject subIconHolder;
    [SerializeField] private Image subIcon;
    [SerializeField] private Sprite[] subIconSprites;

    public float spawnBufferTime = .5f;
    public float despawnTime = 5f;
    public CombatCrystal crystal;
    public Spell defaultAbility;
    public List<Ability> subAbilities = new List<Ability>();

    private bool canBePickedup = false;
    private Camera cam;

    private Coroutine enableCoroutine = null;
    private Coroutine disableCoroutine = null;

    private void OnEnable()
    {
        cam = Camera.main;

        subAbilities.Clear();
        subIcon.CrossFadeAlpha(1f, 0f, true); // Set sub icon to visible

        enableCoroutine = StartCoroutine(EnableSelf(spawnBufferTime));
        disableCoroutine = StartCoroutine(DisableSelf(despawnTime));
    }

    private void Update()
    {
        if (CombatManager.Instance.IsBattleOver)
            return;

        subIconHolder.transform.rotation = Quaternion.LookRotation(
            cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (CombatManager.Instance.IsBattleOver || !CombatManager.Instance.CanCollectCrystals)
            return;

        if (other.tag == "Player" && canBePickedup)
        {
            if (CombatManager.Instance?.GetCollectedCrystalsCount() < 3)
            {
                OnCrystalCollected?.Invoke(crystal);

                // Execute all sub abilities
                foreach (Ability a in subAbilities)
                    a.Target(CombatManager.Instance.GetTargetingManager(), other.GetComponent<IDamagable>());
            }
            else
                return;

            StopAllCoroutines();
            StartCoroutine(DisableSelf(0));
        }
    }

    public void SetSubIcon(int index)
    {
        switch (index)
        {
            case 0: // Attack
                subIcon.sprite = subIconSprites[0];
                break;
            case 1: // Heal
                subIcon.sprite = subIconSprites[1];
                break;
            case 2: // Defense
                break;
            default:
                subIcon.CrossFadeAlpha(0f, 0f, true); // Set sub icon invisible
                break;
        }
    }

    private IEnumerator EnableSelf(float time)
    {
        // Choosing for now to not hook these into CombatFlow, we'll see how that goes...
        yield return new WaitForSeconds(time);
        canBePickedup = true;
    }

    public IEnumerator DisableSelf(float time)
    {
        yield return new WaitForSeconds(time);
        this.transform.parent.gameObject.SetActive(false);
    }

    // TODO CYRENE: REMOVE DEBUG FUNCTION LATER
    public static void ManuallyInvokeCrystalCollected(CombatCrystal c)
    {
        OnCrystalCollected?.Invoke(c);
    }

    public void Init()
    {
        throw new NotImplementedException();
    }
}
