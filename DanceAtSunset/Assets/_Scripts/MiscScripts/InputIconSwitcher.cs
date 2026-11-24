using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InputIconSwitcher : MonoBehaviour
{
    [Header("Keyboard")]
    [SerializeField] private GameObject keyboardText;

    [Header("Gamepad")]
    [SerializeField] private GameObject gamepadImage;

    private void OnEnable()
    {
        InputHandler.OnInputDeviceChanged += UpdateIcon;
        UpdateIcon(InputHandler.IsUsingGamepad);
    }

    private void OnDisable()
    {
        InputHandler.OnInputDeviceChanged -= UpdateIcon;
    }

    private void UpdateIcon(bool usingGamepad)
    {
        Debug.Log($"I am changing because usingGamepad is {usingGamepad}");

        keyboardText.gameObject.SetActive(!usingGamepad);
        gamepadImage.gameObject.SetActive(usingGamepad);
    }
}