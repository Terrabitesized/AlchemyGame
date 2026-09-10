using UnityEngine;

public class OverworldSFXManager : MonoBehaviour
{
    public static OverworldSFXManager Instance;

    [SerializeField] private AudioSource sfxSource;

    [Header("Save Point")]
    [SerializeField] private AudioClip savePointEnter;
    [SerializeField] private AudioClip savePointInteract;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySavePointEnter()
    {
        Debug.Log("Playing Save Point Enter SFX");
        if (savePointEnter != null)
            sfxSource.PlayOneShot(savePointEnter);
    }

    public void PlaySavePointInteract()
    {
        if (savePointInteract != null)
            sfxSource.PlayOneShot(savePointInteract);
    }
}
