using System.Collections;
using UnityEngine;

public class CombatSFXManager : MonoBehaviour
{
    public static CombatSFXManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip[] combatMusic;
    [SerializeField] private AudioClip victoryMusic;

    [Header("Oneshots")]
    [SerializeField] private AudioClip combatStartSfx;
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip spellCastSfx;
    [SerializeField] private AudioClip dashSfx;
    [SerializeField] private AudioSource ingredientSource;
    [SerializeField] private AudioClip enemyAttackPrimedSFX;
    [SerializeField] private AudioSource enemyDamagedSource;
    [SerializeField] private AudioSource playerDamagedSource;
    [SerializeField] private AudioSource playerHealedSource;
    [SerializeField] private AudioSource playerLowDamSource;

    private MusicState currentState;
    private AudioClip currentMusic;

    private Coroutine combatTransitionCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
    }

    private void OnEnable()
    {
        IngredientScript.OnIngredientCollected += PlayIngredientCollected;
        EnemyStats.OnEnemyDamaged += PlayEnemyDamagedSFX;
        PlayerStats.OnPlayerDamaged += (int i, IDamagable d) =>
        {
            PlayPlayerDamagedSFX(i, d);
        };
        BaseEnemyAI.OnEnemyAbilityPrimed += PlayEnemyAttackPrimedSFX;
        CombatManager.OnCombatStart += HandleCombatStarted;
        CombatManager.OnCombatEnd += (bool isVictory) =>
        {
            if (isVictory)
            {
                SetMusicState(MusicState.Victory);
            }
            else
            {
               
            }
        };
    }

    private void OnDisable()
    {
        IngredientScript.OnIngredientCollected -= PlayIngredientCollected;
        EnemyStats.OnEnemyDamaged -= PlayEnemyDamagedSFX;
        PlayerStats.OnPlayerDamaged -= (int i, IDamagable d) =>
        {
            PlayPlayerDamagedSFX(i, d);
        };
        BaseEnemyAI.OnEnemyAbilityPrimed -= PlayEnemyAttackPrimedSFX;
        CombatManager.OnCombatStart -= HandleCombatStarted;
        CombatManager.OnCombatEnd -= (bool isVictory) =>
        {
            if (isVictory)
            {
                SetMusicState(MusicState.Victory);
            }
            else
            {
               
            }
        };
    }

    private void HandleCombatStarted(int numOfEnemies)
    {
        SetMusicState(MusicState.Combat);
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

            case MusicState.Combat:
                sfxSource.PlayOneShot(combatStartSfx);
                PlayMusic(GetRandomCombatMusic());
                break;

            case MusicState.Victory:

                StopCombatTransition();

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

    private AudioClip GetRandomCombatMusic()
{
    if (combatMusic == null || combatMusic.Length == 0)
    {
        Debug.LogWarning("No combat music has been assigned!");
        return null;
    }

    return combatMusic[Random.Range(0, combatMusic.Length)];
}

    public void PlayCombatStartSFX()
    {
        StopCombatTransition();

        combatTransitionCoroutine = StartCoroutine(CombatTransition());
    }

    private IEnumerator CombatTransition()
    {
        sfxSource.PlayOneShot(combatStartSfx);
        yield return new WaitForSeconds(0.05f);

        if (currentState != MusicState.Combat)
        {
            yield break;
        }

        PlayMusic(GetRandomCombatMusic());

        combatTransitionCoroutine = null;
    }

    private void StopCombatTransition()
    {
        if (combatTransitionCoroutine != null)
        {
            StopCoroutine(combatTransitionCoroutine);
            combatTransitionCoroutine = null;
        }
    }

    public void StopAllMusic()
    {
        StopCombatTransition();

        musicSource.Stop();
        currentMusic = null;
    }

    public void PlaySpellCast()
    {
        sfxSource.PlayOneShot(spellCastSfx);
    }

    public void PlayIngredientCollected(CombatIngredient c)
    {
        ingredientSource.pitch = Random.Range(0.975f, 1.025f);

        ingredientSource.PlayOneShot(ingredientSource.clip);
    }

    public void PlayPlayerDamagedSFX(int i, IDamagable d)
    {
        if (i > 0)
        {
            if (i <= 3)
            {
                playerLowDamSource.pitch = Random.Range(0.95f, 1.05f);
                playerLowDamSource.PlayOneShot(playerDamagedSource.clip);
                return;
            }

            playerDamagedSource.pitch = Random.Range(0.95f, 1.05f);
            playerDamagedSource.PlayOneShot(playerDamagedSource.clip);

        }
        else if (i < 0)
        {
            playerHealedSource.pitch = Random.Range(0.95f, 1.05f);
            playerHealedSource.PlayOneShot(playerHealedSource.clip);
        }
    }

    public void PlayDashSfx()
    {
        sfxSource.PlayOneShot(dashSfx);
    }
    
    public void PlayEnemyDamagedSFX(int i, IDamagable d)
    {
        enemyDamagedSource.pitch = Random.Range(0.95f, 1.05f);

        enemyDamagedSource.PlayOneShot(enemyDamagedSource.clip);
    }
    public void PlayEnemyAttackPrimedSFX(GameObject g, EnemyAbility e)
    {
        sfxSource.PlayOneShot(enemyAttackPrimedSFX);
    }
}
