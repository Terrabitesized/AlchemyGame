using UnityEngine;
using UnityEngine.VFX;

public class SavePoint : MonoBehaviour, IInteractable
{
    public KeyCode InteractionKey => KeyCode.F;
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