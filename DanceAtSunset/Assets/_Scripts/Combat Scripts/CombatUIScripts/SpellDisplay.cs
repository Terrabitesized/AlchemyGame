using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellDisplay : MonoBehaviour
{
    [SerializeField] private Image spellImage;
    [SerializeField] private TextMeshProUGUI spellName;
    [SerializeField] private Slider cooldownSlider;
    [SerializeField] private TextMeshProUGUI cooldownNumber;
    private CanvasGroup cooldownNumberAlpha;

    public void Init(Spell spell)
    {
        cooldownNumberAlpha = cooldownNumber.GetComponent<CanvasGroup>();
        cooldownNumberAlpha.alpha = 0f;

        spellImage.sprite = spell.spellIcon;
        spellName.text = spell.spellName;
        cooldownSlider.value = 0f;
        
    }

    public void DisplayCooldown(float cooldownTime)
    {
        StartCoroutine(CooldownCoroutine(cooldownTime));
    }

    private IEnumerator CooldownCoroutine(float cooldownTime)
    {
        cooldownNumber.text = cooldownTime.ToString();
        cooldownNumberAlpha.alpha = 1f;

        float cooldown = cooldownTime;
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            cooldown -= Time.deltaTime;

            if(cooldown > 1f)
                cooldownNumber.text = cooldown.ToString("F0");
            else
                cooldownNumber.text = cooldown.ToString("F1");

            progress += Time.deltaTime / cooldownTime;
            cooldownSlider.value = 1f - progress;
        }

        cooldownNumberAlpha.alpha = 0f;
    }
}
