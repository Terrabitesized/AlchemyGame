using System;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{

    private Camera cam;


    [Header("Animation Variables")]
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightCurve;
    public AnimationCurve frequencyCurve;
    [SerializeField] private float textHeight;
    [SerializeField] private float amplitude = 30f;  // How far it rotates (in degrees)

    private TextMeshProUGUI tmp;
    private float time = 0;
    private Vector3 origin;
    private float spin;
    private float phase;

    private void Awake()
    {
        cam = Camera.main;
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Text animation
        tmp.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));
        transform.localScale = Vector3.one * scaleCurve.Evaluate(time);
        transform.position = new Vector3(origin.x, textHeight + heightCurve.Evaluate(time), origin.z);

        float frequency = frequencyCurve.Evaluate(time);

        phase += frequency * Mathf.PI * 2f * Time.deltaTime;

        spin = Mathf.Sin(phase) * amplitude;

        time += Time.deltaTime;

        Quaternion billboardRotation = Quaternion.LookRotation(
            cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );

        transform.rotation = billboardRotation * Quaternion.Euler(0f, 0f, spin);
    }
}
