using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(
            cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );
    }
}
