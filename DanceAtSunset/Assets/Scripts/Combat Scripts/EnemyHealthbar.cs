using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{

    [SerializeField] private Slider slider;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    public void UpdateHealthBar(float curr, float max)
    {
        slider.value = curr/max;
    }


    void Update()
    {
        transform.parent.rotation = cam.transform.rotation;

        transform.position = target.position + offset;
    }
}
