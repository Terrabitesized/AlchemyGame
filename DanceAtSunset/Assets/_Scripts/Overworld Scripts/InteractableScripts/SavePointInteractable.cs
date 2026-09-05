using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class SavePointInteractable : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Press F to Save";

    [SerializeField] private VisualEffect saveCrystalEffect;
    private int PlayerIsNear = Shader.PropertyToID("PlayerIsNear");

    public void Interact()
    {
        SaveMenu.Instance?.Open();
    }

    public void InteractRangeEnter()
    {
        if (saveCrystalEffect == null)
            return;

        saveCrystalEffect.SetBool(PlayerIsNear, true);
    }

    public void InteractRangeExit()
    {
        if (saveCrystalEffect == null)
            return;

        saveCrystalEffect.SetBool(PlayerIsNear, false);
    }
}