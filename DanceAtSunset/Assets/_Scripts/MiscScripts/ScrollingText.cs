using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    [Range(0f, 30f)]
    public float ScrollSpeed = 2;

    [SerializeField]
    TextMeshProUGUI _originalTextMesh;

    RectTransform _originalRect;
    float _textPreferredWidth;
    readonly LinkedList<RectTransform> _textTransforms = new();
    private RectTransform maskTransform;

    /// <summary>
    /// if you want spaces at the end -> put \t at the end, it will be ignored, but spaces will work
    /// </summary>
    public TextMeshProUGUI TextMesh => _originalTextMesh;

    void Awake()
    {
        if (_originalTextMesh == null)
        {
            enabled = false;
            Debug.LogWarning("Scrolling text was disabled, because no original Text was asigned.");
            return;
        }

        maskTransform = GetComponent<RectTransform>();

        _originalRect = _originalTextMesh.GetComponent<RectTransform>();
        _textPreferredWidth = _originalTextMesh.preferredWidth;

        if (_textPreferredWidth == 0)
        {
            enabled = false;
            Debug.LogWarning("Scrolling text was disabled, because there is no text.");
        }
    }

    void Start()
    {
        _textTransforms.AddFirst(_originalRect);
        CreateClones();
    }

    void Update()
    {
        MoveTransforms();
    }

    /// <summary>
    /// Creates new clones from <see cref="TextMesh"/>.
    /// Call directly after modifying the textMesh of the original.
    /// </summary>
    public void UpdateClones()
    {
        foreach (RectTransform rectTrans in _textTransforms)
        {
            if (rectTrans != _originalRect)
                Destroy(rectTrans.gameObject);
        }

        _textTransforms.Clear();

        _textTransforms.AddFirst(_originalRect);

        _textPreferredWidth = _originalTextMesh.preferredWidth;

        _originalRect.localPosition = new Vector3(0, _originalRect.localPosition.y, _originalRect.localPosition.z);

        if (_textPreferredWidth == 0)
        {
            enabled = false;
            Debug.LogWarning("Scrolling text was disabled, because there is no text.");
            return;
        }
        else enabled = true;

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
        float lastTransPosX = _textTransforms.Last.Value.localPosition.x;

        Vector3 newPos = rectTransform.localPosition;
        newPos.x = lastTransPosX + _textPreferredWidth;

        rectTransform.localPosition = newPos;
    }

    void ReattachFirstTransformAtTheEnd()
    {
        float lastTransPosX = _textTransforms.Last.Value.localPosition.x;

        LinkedListNode<RectTransform> node = _textTransforms.First;

        Vector3 newPos = node.Value.localPosition;
        newPos.x = lastTransPosX + _textPreferredWidth;
        node.Value.localPosition = newPos;

        _textTransforms.RemoveFirst();
        _textTransforms.AddLast(node);
    }

    void CreateClones()
    {
        int clones = CalculateNecessaryClones();

        for (int i = 1; i <= clones; i++)
        {
            RectTransform cloneTransform = Instantiate(_originalRect, transform);
            AttachTransformAtTheEnd(cloneTransform);
            _textTransforms.AddLast(cloneTransform);
        }
    }

    int CalculateNecessaryClones()
    {
        RectTransform maskTransform = GetComponent<RectTransform>();
        return (int)Mathf.Ceil(maskTransform.rect.width / _textPreferredWidth);
    }
}
