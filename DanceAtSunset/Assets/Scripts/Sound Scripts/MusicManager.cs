using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip combatMusic;
    [SerializeField] private AudioClip overworldMusic;
    [SerializeField] private AudioClip victoryMusic;

    [Header("Oneshots")]
    [SerializeField] private AudioClip combatStartSfx;
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip spellCastSfx;
    [SerializeField] private AudioClip ingredientCollectedSfx;

    private MusicState currentState;
    private AudioClip currentMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        IngredientScript.OnIngredientCollected += PlayIngredientCollected;
    }

    private void OnDisable()
    {
        IngredientScript.OnIngredientCollected -= PlayIngredientCollected;
    }

    public enum MusicState
    {
        Overworld,
        Combat,
        Victory
    }

    public void SetMusicState(MusicState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case MusicState.Overworld:
                //PlayMusic(overworldMusic);
                musicSource.Stop();
                break;

            case MusicState.Combat:
                sfxSource.PlayOneShot(combatStartSfx);
                PlayMusic(combatMusic);
                break;

            case MusicState.Victory:
                sfxSource.PlayOneShot(victorySfx);
                PlayMusic(victoryMusic);
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Music clip is null");
            return;
        }
        Debug.Log($"Playing music: {clip.name}");
        if (currentMusic == clip) return;

        currentMusic = clip;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayCombatStartSFX()
    {
        StartCoroutine(CombatTransition());
    }

    private IEnumerator CombatTransition()
    {
        sfxSource.PlayOneShot(combatStartSfx);
        yield return new WaitForSeconds(0.05f);
        PlayMusic(combatMusic);
    }

    public void StopAllMusic()
    {
        musicSource.Stop();
        currentMusic = null;
    }

    public void PlaySpellCast()
    {
        sfxSource.PlayOneShot(spellCastSfx);
    }

    public void PlayIngredientCollected(CombatIngredient c)
    {
        sfxSource.PlayOneShot(ingredientCollectedSfx);

    }
}
