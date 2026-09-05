using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{

    [SerializeField] private Slider slider;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        cam = Camera.main;
        target = transform.root;
    }

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
