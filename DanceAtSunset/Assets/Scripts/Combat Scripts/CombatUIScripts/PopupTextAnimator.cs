using System;
using System.Collections;
using Alchemy.Inspector;
using TMPro;
using UnityEditor;
using UnityEngine;
public class PopupTextAnimator : MonoBehaviour
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
    private float spin;
    private float phase;
    private float duration;

    private void Awake()
    {
        cam = Camera.main;
        tmp = GetComponentInChildren<TextMeshProUGUI>();

        transform.position = new Vector3(transform.position.x, textHeight, transform.position.z);
    }

    private void OnEnable()
    {
        transform.position = new Vector3(transform.position.x, textHeight, transform.position.z);
        StartCoroutine(Animate());
    }

    private void OnDisable()
    {
        time = 0;
        phase = 0;
    }

    public void Init(float duration)
    {
        this.duration = duration;
        Invoke("DisableSelf", duration);
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

        if (animateFrequency)
        {
            float frequency = frequencyCurve.Evaluate(time);
            phase += frequency * Mathf.PI * 2f * Time.deltaTime;
            spin = Mathf.Sin(phase) * amplitude;

            transform.rotation = billboardRotation * Quaternion.Euler(0f, 0f, spin);
        }

        time += Time.deltaTime;
    }

    private IEnumerator Animate()
    {
        // Lerp scale up to simulate the ship flying up to planet
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / duration;

            // Text animation
            if (animateOpacity)
                tmp.color = new Color(1, 1, 1, opacityCurve.Evaluate(progress));

            if (animateScale)
                transform.localScale = Vector3.one * scaleCurve.Evaluate(progress);

            if (animateHeight)
                transform.position = new Vector3(transform.position.x, textHeight + heightCurve.Evaluate(progress), transform.position.z);
        }
    }

    void DisableSelf()
    {
        this.gameObject.SetActive(false);
    }
}
