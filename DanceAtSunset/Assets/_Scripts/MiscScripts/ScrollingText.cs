using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    [Range(0f, 30f)]
    public float ScrollSpeed = 2;

    public TextMeshProUGUI OriginalTextMesh;

    RectTransform _originalRect;
    float _textPreferredWidth;
    readonly LinkedList<RectTransform> _textTransforms = new();
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
            maskTransform.rect.xMin,
            _originalRect.localPosition.y,
            _originalRect.localPosition.z
        );

        if (_textPreferredWidth <= 0)
        {
            enabled = false;
            return;
        }

        enabled = true;
        CreateClones();
    }

    void MoveTransforms()
    {
        float distance = ScrollSpeed * 30 * Time.deltaTime;

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
