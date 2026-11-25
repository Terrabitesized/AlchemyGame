using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    [Range(0f, 30f)]
    public float MinscrollSpeed = 2;
    public float MaxscrollSpeed = 10;
    private float scrollSpeed;

    public AnimationCurve TextSpeedDecayCurve;
    public float TextSpeedDecayTime = .75f;
    private Coroutine textSpeedDecayCoroutine;

    public TextMeshProUGUI OriginalTextMesh;

    [Header("Animation Parameters")]
    public bool FadeTextOnChange = true;
    [ShowIf(nameof(FadeTextOnChange))]
    [SerializeField] private AnimationCurve fadeCurve;
    [ShowIf(nameof(FadeTextOnChange))]
    [SerializeField] private float fadeInTime = .25f;
    private CanvasGroup maskCanvasGroup;
    private Coroutine fadeCoroutine;

    private RectTransform _originalRect;
    private float _textPreferredWidth;
    private readonly LinkedList<RectTransform> _textTransforms = new();
    private RectTransform maskTransform;
    

    /// <summary>
    /// if you want spaces at the end -> put \t at the end, it will be ignored, but spaces will work
    /// </summary>
    public TextMeshProUGUI TextMesh => OriginalTextMesh;

    void Awake()
    {
        if (OriginalTextMesh == null)
        {
            enabled = false;
            Debug.LogWarning("Scrolling text was disabled, because no original Text was asigned.");
            return;
        }

        maskTransform = GetComponent<RectTransform>();
        maskCanvasGroup = GetComponent<CanvasGroup>();

        _originalRect = OriginalTextMesh.GetComponent<RectTransform>();
        _textPreferredWidth = OriginalTextMesh.preferredWidth;

        if (_textPreferredWidth == 0)
        {
            enabled = false;
            Debug.LogWarning("Scrolling text was disabled, because there is no text.");
        }
    }

    IEnumerator Start()
    {
        _textTransforms.AddFirst(_originalRect);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(maskTransform);

        CreateClones();
        yield break;
    }

    void Update()
    {
        MoveTransforms();
    }

    /// <summary>
    /// Creates new clones from <see cref="TextMesh"/>.
    /// Call directly after modifying the textMesh of the original.
    /// </summary>
    public void UpdateClones(string text)
    {
        if(FadeTextOnChange)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeText());
        }

        OriginalTextMesh.text = text;
        OriginalTextMesh.ForceMeshUpdate();

        Canvas.ForceUpdateCanvases();

        _textPreferredWidth = OriginalTextMesh.preferredWidth;

        foreach (RectTransform rectTrans in _textTransforms)
        {
            if (rectTrans != _originalRect)
                Destroy(rectTrans.gameObject);
        }

        _textTransforms.Clear();
        _textTransforms.AddFirst(_originalRect);

        _originalRect.localPosition = new Vector3(
            maskTransform.rect.xMax,
            _originalRect.localPosition.y,
            _originalRect.localPosition.z
        );

        if (_textPreferredWidth <= 0)
        {
            enabled = false;
            return;
        }

        if(textSpeedDecayCoroutine != null)
            StopCoroutine(textSpeedDecayCoroutine);

        textSpeedDecayCoroutine = StartCoroutine(TextSpeedDecay());

        enabled = true;
        CreateClones();
    }

    private IEnumerator TextSpeedDecay()
    {
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / TextSpeedDecayTime;

            scrollSpeed = Mathf.Lerp(MaxscrollSpeed, MinscrollSpeed, TextSpeedDecayCurve.Evaluate(progress));
        }

        textSpeedDecayCoroutine = null;
    }

    private IEnumerator FadeText()
    {
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / fadeInTime;

            maskCanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeCurve.Evaluate(progress));
        }

        fadeCoroutine = null;
    }

    void MoveTransforms()
    {
        float distance = scrollSpeed * 30 * Time.deltaTime;

        foreach (RectTransform transform in _textTransforms)
        {
            Vector3 newPos = transform.localPosition;
            newPos.x -= distance;
            transform.localPosition = newPos;
        }

        CheckIfLeftMostTransformLeftMask();
    }

    void CheckIfLeftMostTransformLeftMask()
    {
        RectTransform rectTransform = _textTransforms.First.Value;

        float rightEdge = rectTransform.localPosition.x
            + (_textPreferredWidth * (1f - rectTransform.pivot.x));

        if (rightEdge <= maskTransform.rect.xMin)
        {
            ReattachFirstTransformAtTheEnd();
        }
    }

    void AttachTransformAtTheEnd(RectTransform rectTransform)
    {
        RectTransform lastTransform = _textTransforms.Last.Value;

        float lastRightEdge = lastTransform.localPosition.x + _textPreferredWidth;

        Vector3 newPos = rectTransform.localPosition;
        newPos.x = lastRightEdge;

        rectTransform.localPosition = newPos;
    }

    void ReattachFirstTransformAtTheEnd()
    {
        LinkedListNode<RectTransform> node = _textTransforms.First;

        AttachTransformAtTheEnd(node.Value);

        _textTransforms.RemoveFirst();
        _textTransforms.AddLast(node);
    }

    void CreateClones()
    {
        int clones = CalculateNecessaryClones();

        Debug.Log(
            $"Mask Width: {maskTransform.rect.width}, " +
            $"Text Width: {_textPreferredWidth}, " +
            $"Clones: {clones}"
        );

        for (int i = 0; i < clones; i++)
        {
            RectTransform cloneTransform = Instantiate(_originalRect, transform);
            AttachTransformAtTheEnd(cloneTransform);
            _textTransforms.AddLast(cloneTransform);
        }
    }

    int CalculateNecessaryClones()
    {
        return Mathf.CeilToInt(maskTransform.rect.width / _textPreferredWidth) + 1;
    }
}
