using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CrystalAura : MonoBehaviour
{
    [SerializeField] private CrystalProjectile[] crystals;
    private float radius = 1f;
    private float rotationSpeed = 100f;

    private int currentCrystalCount = 0;

    private void OnEnable()
    {
        IngredientScript.OnIngredientCollected += AddCrystal;
        CombatManager.OnSpellCast += FireCrystals;
        CombatManager.OnCrystalsManuallyCleared += ClearCrystals;
    }

    private void OnDisable()
    {
        IngredientScript.OnIngredientCollected -= AddCrystal;
        CombatManager.OnSpellCast -= FireCrystals;
        CombatManager.OnCrystalsManuallyCleared -= ClearCrystals;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void AddCrystal(CombatCrystal crystal)
    {
        crystals[currentCrystalCount].Initialize(crystal);

        currentCrystalCount++;

        for(int i = 0; i < currentCrystalCount; i++)
        {
            float angle = i * Mathf.PI * 2f / currentCrystalCount;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            crystals[i].gameObject.transform.localPosition = new Vector3(x, 0f, z);
        }
    }

    private void FireCrystals()
    {
        foreach (CrystalProjectile projectile in crystals)
            projectile.Fire();
    }

    private void ClearCrystals(Spell s) { ClearCrystals(); }

    private void ClearCrystals()
    {
        //foreach (VisualEffect crystal in crystals)
        //    crystal.gameObject.SetActive(false);

        currentCrystalCount = 0;
    }
}
