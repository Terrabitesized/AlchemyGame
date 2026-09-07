using System;
using System.Collections;
using Alchemy.Inspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class AbilityPopupAnimator : MonoBehaviour
{
    [SerializeField] private Image sliderBorder;
    [SerializeField] private Image sliderFill;

    [Header("Animation Variables")]
    public bool animateOpacity = false;
    [ShowIf(nameof(animateOpacity))] public AnimationCurve opacityCurve;
    public float debuildDuration = .5f;
    public float textHeight;

    private Camera cam;
    private TextMeshProUGUI tmp;
    private Slider slider;
    private CanvasGroup canvasGroup;
    private float duration;

    private void Awake()
    {
        cam = Camera.main;
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        slider = GetComponentInChildren<Slider>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (slider != null)
            slider.value = 0f;

        if(canvasGroup != null)
            canvasGroup.alpha = 1f;

        StartCoroutine(Animate());
    }

    public void Init(float duration, string abilityName)
    {
        GetComponent<RectTransform>().localPosition = new Vector3(0f, textHeight, 0f);
        tmp.text = abilityName;

        this.duration = duration;
        Invoke("DisableSelf", duration + debuildDuration);
    }

    // Update is called once per frame
    void Update()
    {
        // Billboard text
        transform.rotation = Quaternion.LookRotation(
            cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );
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
                canvasGroup.alpha = opacityCurve.Evaluate(progress);

            slider.value = progress;
        }

        progress = 0f;
        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / debuildDuration;

            canvasGroup.alpha = 1f - progress;
        }
    }

    void DisableSelf()
    {
        this.gameObject.SetActive(false);
    }
}
