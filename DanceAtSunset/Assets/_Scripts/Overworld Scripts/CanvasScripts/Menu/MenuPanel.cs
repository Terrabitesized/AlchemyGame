using UnityEngine;
using UnityEngine.UI;

public abstract class MenuPanel : MonoBehaviour
{
    [Header("Base Fields")]
    public MenuState MenuState;
    public string MenuName;
    public Button PanelButton;
    protected PlayerDataBinding[] dataBindings;

    public abstract void Show();

    public abstract void Hide();
}
