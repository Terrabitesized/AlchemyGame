using System;
using TMPro;
using UnityEngine;

public class CombatPopupManager : MonoBehaviour
{
    private GameObject player;

    [Header("Damage Popups")]
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private float damagePopupDuration;

    [Header("Ability Usage Popups")]
    [SerializeField] private GameObject abilityUsageopupPrefab;
    [SerializeField] private float abilityUsagePopupDuration;

    private void OnEnable()
    {
        EnemyStats.OnEnemyDamaged += CreateDamagePopUp;
        PotionManager.OnSpellCast += CreateAbilityUsagePopUp;
    }

    private void OnDisable()
    {
        EnemyStats.OnEnemyDamaged -= CreateDamagePopUp;
        PotionManager.OnSpellCast -= CreateAbilityUsagePopUp;
    }

    public void CreateDamagePopUp(int damage, IDamagable damagedTarget)
    {
        GameObject damagePopup = null;

        if (damagedTarget is Component component)
        {
            damagePopup = Instantiate(damagePopupPrefab, component.gameObject.transform.position, Quaternion.identity);
            var temp = damagePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            temp.text = damage.ToString();
        }

        //Destroy Timer
        Destroy(damagePopup, damagePopupDuration);
    }

    public void CreateAbilityUsagePopUp(Spell spell)
    {
        GameObject abilityPopup = null;

        Debug.Log("HELLO I SHOULD BE MAKING AN ABILITY????");

        if(player == null)
            player = CombatManager.Instance?.GetPlayerGameObject();

        if (player != null)
        {
            Debug.Log("HELLO I SHOULD BE MAKING AN ABILITY????");

            abilityPopup = Instantiate(abilityUsageopupPrefab, player.transform.position, Quaternion.identity);
            var temp = abilityPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            temp.text = spell.spellName.ToString();
        }
        else
        {
            Debug.Log("PLAYER WAS SOMEHOW NULL");
        }

        //Destroy Timer
        if(spell.spellAbility.requiresCasting)
            Destroy(abilityPopup, spell.spellAbility.castDuration);
        else
            Destroy(abilityPopup, .5f);

    }
}