using UnityEngine;

public class OverworldSFXManager : MonoBehaviour
{
    public static OverworldSFXManager Instance;

    [SerializeField] private AudioSource sfxSource;

    [Header("Save Point")]
    [SerializeField] private AudioClip savePointEnter;
    [SerializeField] private AudioClip savePointInteract;

    [Header("Menu")]
    [SerializeField] private AudioClip menuOpen;
    [SerializeField] private AudioClip menuClose;
    [SerializeField] private AudioClip changeMenuTab;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        OverworldMenuManager.OnMenuOpened += PlayMenuOpened;
        OverworldMenuManager.OnMenuClosed += PlayMenuClosed;
        OverworldMenuManager.OnMenuTabChanged += PlayMenuTabChanged;
    }

    private void OnDisable()
    {
        OverworldMenuManager.OnMenuOpened -= PlayMenuOpened;
        OverworldMenuManager.OnMenuClosed -= PlayMenuClosed;
        OverworldMenuManager.OnMenuTabChanged -= PlayMenuTabChanged;
    }

    public void PlaySavePointEnter()
    {
        Debug.Log("Playing Save Point Enter SFX");
        if (sfxSource != null && savePointEnter != null)
            sfxSource.PlayOneShot(savePointEnter);
    }

    public void PlaySavePointInteract()
    {
        if (sfxSource != null && savePointInteract != null)
            sfxSource.PlayOneShot(savePointInteract);
    }

    private void PlayMenuTabChanged()
    {
        sfxSource?.PlayOneShot(changeMenuTab);
    }

    private void PlayMenuOpened()
    {
        sfxSource?.PlayOneShot(menuOpen);
    }

    private void PlayMenuClosed()
    {
        sfxSource?.PlayOneShot(menuClose);
    }
}
