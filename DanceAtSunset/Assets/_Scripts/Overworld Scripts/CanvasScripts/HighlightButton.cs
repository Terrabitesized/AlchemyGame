using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    private static HighlightButton lastSelectedButton;

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"Button {gameObject.name} was selected!");
        lastSelectedButton = this;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log($"Button {gameObject.name} was deselected!");
        StartCoroutine(CheckSelection());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    private IEnumerator CheckSelection()
    {
        yield return new WaitForEndOfFrame();

        if (!isActiveAndEnabled)
            yield break;

        if (EventSystem.current.currentSelectedGameObject != null)
            yield break;

        if (lastSelectedButton == null ||
            !lastSelectedButton.isActiveAndEnabled)
            yield break;

        EventSystem.current.SetSelectedGameObject(
            lastSelectedButton.gameObject
        );
    }

    private void OnDisable()
    {
        if (lastSelectedButton == this)
            lastSelectedButton = null;
    }
}
