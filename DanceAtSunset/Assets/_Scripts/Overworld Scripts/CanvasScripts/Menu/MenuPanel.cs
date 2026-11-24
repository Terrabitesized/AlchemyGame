using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private MenuState menuState;
    [SerializeField] private string menuText;
    [SerializeField] private Button panelButton;
    [SerializeField] private ScrollingText menuTextScroll;

    public void Show()
    {
        // Set scrolling text
        menuTextScroll.UpdateClones(menuText);

        // Highlight selected button
        panelButton.image.color = new Color32(109, 169, 146, 255);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        // Unhighlight selected button
        panelButton.image.color = new Color32(73, 86, 81, 255);

        gameObject.SetActive(false);
    }
}
