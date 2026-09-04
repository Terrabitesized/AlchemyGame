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
        damagePopup.GetComponent<PopupAnimator>()?.Init(damagePopupDuration);
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
            var temp = abilityPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            temp.text = spell.spellName.ToString();
        }

        abilityPopup.SetActive(true);

        // Return to pool
        if (spell.spellAbility.requiresCasting)
            abilityPopup.GetComponent<PopupAnimator>()?.Init(spell.spellAbility.castDuration);
        else
            abilityPopup.GetComponent<PopupAnimator>()?.Init(.5f);

    }

    public void CreateEnemyAbilityUsagePopUp(GameObject enemy, EnemyAbility enemyAbility)
    {
        // Attempts to grab an ingredient from the pool
        GameObject abilityPopup = CombatObjectPool.Instance.GetPooledAbilityPopup();

        abilityPopup.transform.position = enemy.transform.position;
        var temp = abilityPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = enemyAbility.enemyAttackPattern.AttackName.ToString();

        abilityPopup.SetActive(true);

        // Return to pool
        abilityPopup.GetComponent<PopupAnimator>()?.Init(enemyAbility.enemyAttackPattern.AttackCastTime);

        Debug.Log($"{enemy.name} is using {enemyAbility.enemyAttackPattern.AttackName}!");
    }
}