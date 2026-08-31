using System;
using Alchemy.Inspector;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PopupAnimator : MonoBehaviour
{
    private Camera cam;

    [Header("Animation Variables")]
    public bool animateOpacity = false;
    [ShowIf(nameof(animateOpacity))] public AnimationCurve opacityCurve;
    public bool animateScale = false;
    [ShowIf(nameof(animateScale))] public AnimationCurve scaleCurve;
    public bool animateHeight = false;
    [ShowIf(nameof(animateHeight))] public AnimationCurve heightCurve;
    public bool animateFrequency = false;
    [ShowIf(nameof(animateFrequency))] public AnimationCurve frequencyCurve;
    [ShowIf(nameof(animateFrequency))] public float amplitude = 30f;
    public float textHeight;

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

        transform.position = new Vector3(origin.x, textHeight, origin.z);
    }

    private void OnDisable()
    {
        time = 0;
        phase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Billboard text
        Quaternion billboardRotation = Quaternion.LookRotation(
            cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );

        transform.rotation = billboardRotation;

        // Text animation
        if (animateOpacity)
            tmp.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));

        if(animateScale)
            transform.localScale = Vector3.one * scaleCurve.Evaluate(time);

        if (animateHeight)
            transform.position = new Vector3(origin.x, textHeight + heightCurve.Evaluate(time), origin.z);

        if(animateFrequency)
        {
            float frequency = frequencyCurve.Evaluate(time);
            phase += frequency * Mathf.PI * 2f * Time.deltaTime;
            spin = Mathf.Sin(phase) * amplitude;

            transform.rotation = billboardRotation * Quaternion.Euler(0f, 0f, spin);
        }

        time += Time.deltaTime;
    }
}
