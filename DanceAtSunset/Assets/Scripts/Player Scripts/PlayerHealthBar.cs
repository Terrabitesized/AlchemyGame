using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    //[SerializeField] private Slider easeHealthSlider;
    //private float healthlerpSpeed = 0.01f;
    private float lastVal = 100;
    public TextMeshProUGUI hpText;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        hpText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void UpdateHealthBar(float curr, float max)
    {
        lastVal = curr;
        slider.value = curr / max;
        hpText.text = "hp: " + curr +"/" + max;

    }

    //ugh. currently unused.
    void Update()
    {
        //easeHealthSlider.value = Mathf.Lerp(slider.value, lastVal, healthlerpSpeed);
    }
}
