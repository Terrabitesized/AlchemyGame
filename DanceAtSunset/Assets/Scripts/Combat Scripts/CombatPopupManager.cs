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

    private void OnEnable()
    {
        EnemyStats.OnEnemyDamaged += CreateDamagePopUp;
        PotionManager.OnSpellCast += CreateAbilityUsagePopUp;
        BaseEnemyAI.OnEnemyAbilityPrimed += CreateEnemyAbilityUsagePopUp;
    }

    private void OnDisable()
    {
        EnemyStats.OnEnemyDamaged -= CreateDamagePopUp;
        PotionManager.OnSpellCast -= CreateAbilityUsagePopUp;
        BaseEnemyAI.OnEnemyAbilityPrimed -= CreateEnemyAbilityUsagePopUp;
    }

    public void CreateDamagePopUp(int damage, IDamagable damagedTarget)
    {
        // Attempts to grab an ingredient from the pool
        GameObject damagePopup = CombatObjectPool.Instance.GetPooledDamagePopup();

        if (damagedTarget is Component component)
        {
            damagePopup.transform.position = component.gameObject.transform.position;
            var temp = damagePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            temp.text = damage.ToString();
        }

        damagePopup.SetActive(true);

        // Return to pool
        damagePopup.GetComponent<PopupTextAnimator>()?.Init(damagePopupDuration);
    }

    public void CreateAbilityUsagePopUp(Spell spell)
    {
        // Attempts to grab an ingredient from the pool
        GameObject abilityPopup = CombatObjectPool.Instance.GetPooledAbilityPopup();

        Debug.Log("HELLO I SHOULD BE MAKING AN ABILITY????");

        if(player == null)
            player = CombatManager.Instance?.GetPlayerGameObject();

        if (player != null)
        {
            Debug.Log("HELLO I SHOULD BE MAKING AN ABILITY????");

            abilityPopup.transform.position = player.transform.position;
            var temp = abilityPopup.transform.GetComponentInChildren<TextMeshProUGUI>();
            temp.text = spell.spellName.ToString();
        }

        abilityPopup.SetActive(true);

        // Return to pool
        if (spell.spellAbility.requiresCasting)
            abilityPopup.GetComponent<AbilityPopupAnimator>()?.Init(spell.spellAbility.castDuration, player.transform);
        else
            abilityPopup.GetComponent<AbilityPopupAnimator>()?.Init(.5f, player.transform);

    }

    public void CreateEnemyAbilityUsagePopUp(GameObject enemy, EnemyAbility enemyAbility)
    {
        // Attempts to grab an ingredient from the pool
        GameObject abilityPopup = CombatObjectPool.Instance.GetPooledAbilityPopup();

        abilityPopup.transform.position = enemy.transform.position;
        var temp = abilityPopup.transform.GetComponentInChildren<TextMeshProUGUI>();
        temp.text = enemyAbility.enemyAttackPattern.AttackName.ToString();

        abilityPopup.SetActive(true);

        // Return to pool
        abilityPopup.GetComponent<AbilityPopupAnimator>()?.Init(enemyAbility.enemyAttackPattern.AttackCastTime, enemy.transform);

        Debug.Log($"{enemy.name} is using {enemyAbility.enemyAttackPattern.AttackName}!");
    }
}