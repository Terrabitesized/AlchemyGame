using System;
using UnityEngine;
using UnityEngine.VFX;

public class IngredientAura : MonoBehaviour
{
    [SerializeField] private VisualEffect[] crystals;
    private int currentIngredientCount = 0;

    private float radius = 1f;
    private float rotationSpeed = 100f;

    private int INGREDIENT_COLOR = Shader.PropertyToID("IngredientColor");

    private void OnEnable()
    {
        IngredientScript.OnIngredientCollected += AddCrystal;
        PotionManager.OnSpellCast += ClearCrystals;
        CombatManager.OnIngredientsManuallyCleared += ClearCrystals;
    }

    private void OnDisable()
    {
        IngredientScript.OnIngredientCollected -= AddCrystal;
        PotionManager.OnSpellCast -= ClearCrystals;
        CombatManager.OnIngredientsManuallyCleared -= ClearCrystals;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void AddCrystal(CombatIngredient ingredient)
    {
        crystals[currentIngredientCount].SetVector4(INGREDIENT_COLOR, ingredient.color);
        currentIngredientCount++;

        for(int i = 0; i < currentIngredientCount; i++)
        {
            float angle = i * Mathf.PI * 2f / currentIngredientCount;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            crystals[i].gameObject.transform.localPosition = new Vector3(x, 0f, z);

            crystals[i].gameObject.SetActive(true);
        }
    }

    private void ClearCrystals(Spell s) { ClearCrystals(); }

    private void ClearCrystals()
    {
        foreach (VisualEffect crystal in crystals)
            crystal.gameObject.SetActive(false);

        currentIngredientCount = 0;
    }
}
