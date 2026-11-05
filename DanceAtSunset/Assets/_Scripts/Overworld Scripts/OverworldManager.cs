using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    public static OverworldManager Instance;

    public string CurrentZoneCombatSceneName = "FORREST_COMBAT";

    private GameObject player;
    [SerializeField] private AudioSource musicSource;

    [Header("Music")]
    [SerializeField] private AudioClip overworldMusic;

    private AudioClip currentMusic;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Get reference to player
        player = GameObject.FindGameObjectWithTag("Player");

        // Check if the player is returning from combat
        if (StaticOverworldData.loadingFromCombat)
        {
            Debug.Log("WE ARE LOADING FROM COMBAT");

            if (player != null)
            {
                CharacterController character =
                    player.GetComponent<CharacterController>();

                if (character != null)
                    character.enabled = false;

                // Restore position and rotation
                player.transform.position = StaticOverworldData.playerPosition;
                player.transform.rotation = StaticOverworldData.playerRotation;

                if (character != null)
                    character.enabled = true;

                // Restore player stats
                OverworldStats overworldStats = player.GetComponent<OverworldStats>();

                if (overworldStats != null && StaticCombatData.BaseStats != null)
                {
                    overworldStats.stats = StaticCombatData.BaseStats;
                    overworldStats.UpdateStats();

                    StaticCombatData.BaseStats = null;
                }
            }

            StaticOverworldData.loadingFromCombat = false;
        }

        // Music
        PlayMusic(overworldMusic);
    }

    private void OnDisable()
    {
        player = null;
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("Music source is missing!");
            return;
        }

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

    public GameObject GetPlayer()
    {
        if(player == null)
            return GameObject.FindGameObjectWithTag("Player");

        return player;
    }
}
