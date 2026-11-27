using System.Collections;
using Alchemy.Inspector;
using UnityEngine;

public class OverworldMenuPanel : MenuPanel
{
    [Header("OverworldMenuPanel Fields")]
    [SerializeField] private ScrollingText menuTextScroll;

    [Header("Animation Fields")]
    [SerializeField] private bool animatesIn = false;
    [ShowIf(nameof(animatesIn))] [SerializeField] private AnimationCurve animationCurve;
    [ShowIf(nameof(animatesIn))] [SerializeField] private float animationDuration = .25f;
    private CanvasGroup panelCanvasGroup;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        panelCanvasGroup = GetComponent<CanvasGroup>();
        dataBindings = GetComponentsInChildren<PlayerDataBinding>();
    }

    public override void Show()
    {
        // Set scrolling text
        menuTextScroll.UpdateClones(MenuName);

        // Highlight selected button
        PanelButton.image.color = new Color32(125, 195, 239, 255);

        gameObject.SetActive(true);

        if(animatesIn)
            animationCoroutine = StartCoroutine(FadeCoroutine());

        foreach(PlayerDataBinding binding in dataBindings)
            binding.Refresh();
    }

    public override void Hide()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        // Unhighlight selected button
        PanelButton.image.color = new Color32(0, 50, 79, 255);

        gameObject.SetActive(false);
    }

    private IEnumerator FadeCoroutine()
    {
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / animationDuration;

            panelCanvasGroup.alpha = animationCurve.Evaluate(progress);
        }

        animationCoroutine = null;
    }
}
