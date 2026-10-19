using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CrystalAura : MonoBehaviour
{
    [SerializeField] private float crystalFireDelay = .5f;
    private List<CrystalProjectile> crystals = new List<CrystalProjectile>();
    private float radius = 1f;
    private float rotationSpeed = 100f;

    private int currentCrystalCount = 0;

    private void OnEnable()
    {
        IngredientScript.OnCrystalCollected += AddCrystal;
        CombatManager.OnSpellCast += FireCrystals;
        CombatManager.OnCrystalsManuallyCleared += ClearCrystals;
    }

    private void OnDisable()
    {
        IngredientScript.OnCrystalCollected -= AddCrystal;
        CombatManager.OnSpellCast -= FireCrystals;
        CombatManager.OnCrystalsManuallyCleared -= ClearCrystals;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void AddCrystal(CombatCrystal crystal)
    {
        CrystalProjectile crystalProjectile = CombatObjectPool.Instance.
            GetPooledCrystalProjectile().GetComponent<CrystalProjectile>();

        if(crystalProjectile == null)
        {
            Debug.Log("OH MY GOD HTE PROJECTILE IS NULLLLLLLLLL");
        }

        crystalProjectile.gameObject.SetActive(true);
        crystals.Add(crystalProjectile);
        crystalProjectile.Initialize(crystal, crystals.Count - 2);
        crystalProjectile.transform.SetParent(transform);

        for(int i = 0; i < crystals.Count; i++)
        {
            float angle = i * Mathf.PI * 2f / crystals.Count;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            crystals[i].gameObject.transform.localPosition = new Vector3(x, 0f, z);
        }
    }

    private void FireCrystals()
    {
        StartCoroutine(FireCrystalDelay());
    }

    private IEnumerator FireCrystalDelay()
    {
        yield return new WaitForSeconds(crystalFireDelay);

        foreach (CrystalProjectile projectile in crystals)
            projectile.Fire();

        ClearCrystals();
    }

    private void ClearCrystals()
    {
        crystals.Clear();
        currentCrystalCount = 0;
    }
}
