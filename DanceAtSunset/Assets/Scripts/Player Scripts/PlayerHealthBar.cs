using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateHealthBar(float curr, float max)
    {
        slider.value = curr / max;
    }
}
