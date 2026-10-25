using System;
using System.Collections;
using UnityEngine;

public class SpellCrystal : MonoBehaviour, ICombatSpawnable
{
    public static Action OnSpellCrystalCollected;

    public float spawnBufferTime = .5f;
    public float despawnTime = 5f;

    private Coroutine enableCoroutine = null;
    private Coroutine disableCoroutine = null;

    private void OnEnable()
    {
        enableCoroutine = StartCoroutine(EnableSelf(spawnBufferTime));
        disableCoroutine = StartCoroutine(DisableSelf(despawnTime));
    }

    public void Init()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator EnableSelf(float time)
    {
        // Choosing for now to not hook these into CombatFlow, we'll see how that goes...
        yield return new WaitForSeconds(time);
    }

    public IEnumerator DisableSelf(float time)
    {
        yield return new WaitForSeconds(time);
        this.transform.parent.gameObject.SetActive(false);
    }

    public void Collect()
    {
        if (CombatManager.Instance.IsBattleOver || !CombatManager.Instance.CanCollectCrystals)
            return;

        OnSpellCrystalCollected?.Invoke();

        // TODO CYRENE: Implement functionality

        StopAllCoroutines();
        StartCoroutine(DisableSelf(0));
    }
}
