using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private float healthHeight;

    private void Awake()
    {
        transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(0f, healthHeight, 0f);
        cam = Camera.main;
    }

    // TODO CYRENE: Make this animated !!

    public void UpdateHealthBar(float curr, float max)
    {
        healthSlider.value = curr > 0 ? curr/max : 0f;
    }

    public void UpdateShieldBar(float curr, float max)
    {
        shieldSlider.value = curr > 0 ? curr / max : 0f;
    }

    void Update()
    {
        transform.parent.rotation = cam.transform.rotation;
    }
}
