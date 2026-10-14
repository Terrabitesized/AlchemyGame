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
    [SerializeField] private Color playerBorderColor;
    [SerializeField] private Color playerFillColor;
    [SerializeField] private Color enemyBorderColor;
    [SerializeField] private Color enemyFillColor;

    private void OnEnable()
    {
        PlayerStats.OnPlayerDamaged += CreateDamagePopUp;
        EnemyStats.OnEnemyDamaged += CreateDamagePopUp;
        EnemyStats.OnEnemyShieldDamaged += CreateShieldDamagePopup;
        //PotionManager.OnSpellCast += CreateAbilityUsagePopUp;
        //BaseEnemyAI.OnEnemyAbilityPrimed += CreateEnemyAbilityUsagePopUp;
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerDamaged -= CreateDamagePopUp;
        EnemyStats.OnEnemyDamaged -= CreateDamagePopUp;
        EnemyStats.OnEnemyShieldDamaged -= CreateShieldDamagePopup;
        //PotionManager.OnSpellCast -= CreateAbilityUsagePopUp;
        //BaseEnemyAI.OnEnemyAbilityPrimed -= CreateEnemyAbilityUsagePopUp;
    }

    public void CreateDamagePopUp(int damage, IDamagable damagedTarget)
    {
        if (damage == 0)
            return;

        // Attempts to grab a popup from the pool
        GameObject damagePopup = CombatObjectPool.Instance.GetPooledDamagePopup();
        TextMeshProUGUI damagePopupText = damagePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        damagePopupText.color = Color.white;

        if (damagedTarget is PlayerStats player)
        {
            damagePopup.transform.position = player.gameObject.transform.position;

            if(damage < 0)
            {
                damagePopupText.color = Color.green;
                damagePopupText.text = "+" + (damage * -1).ToString();
            }
            else
            {
                damagePopupText.color = Color.red;
                damagePopupText.text = damage.ToString();
            }
        }
        else if (damagedTarget is EnemyStats enemy)
        {
            damagePopup.transform.position = enemy.gameObject.transform.position;

            damagePopupText.text = damage.ToString();
        }

        damagePopup.SetActive(true);

        // Return to pool
        damagePopup.GetComponent<PopupTextAnimator>()?.Init(damagePopupDuration);
    }

    public void CreateShieldDamagePopup(int damage, IDamagable damagedTarget)
    {
        if (damage == 0)
            return;

        // Attempts to grab a popup from the pool
        GameObject damagePopup = CombatObjectPool.Instance.GetPooledDamagePopup();
        TextMeshProUGUI damagePopupText = damagePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        damagePopupText.color = Color.white;

        if (damagedTarget is PlayerStats player)
        {
            damagePopup.transform.position = player.gameObject.transform.position;

            damagePopupText.color = Color.gray;
            damagePopupText.text = damage.ToString();
        }
        else if (damagedTarget is EnemyStats enemy)
        {
            damagePopup.transform.position = enemy.gameObject.transform.position;

            damagePopupText.color = Color.gray;
            damagePopupText.text = damage.ToString();
        }

        damagePopup.SetActive(true);

        // Return to pool
        damagePopup.GetComponent<PopupTextAnimator>()?.Init(damagePopupDuration);
    }
}