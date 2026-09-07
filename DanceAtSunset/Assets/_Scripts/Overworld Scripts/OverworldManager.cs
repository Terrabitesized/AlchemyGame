using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    public static OverworldManager Instance;

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

        // Check if the player is loading from combat
        if(StaticOverworldData.loadingFromCombat)
        {
            Debug.Log("WE ARE LOADING FROM COMBAT");

            StaticOverworldData.loadingFromCombat = false;

            if (player != null)
            {
                player.transform.position = StaticOverworldData.playerPosition;
                player.transform.rotation = StaticOverworldData.playerRotation;
            }
        }

        else if (StaticOverworldData.loadFromMainMenu)
        {
            Debug.Log("WE ARE LOADING FROM MAIN MENU");

            if (player != null)
            {
                player.GetComponent<OverworldStats>().LoadFromJson(StaticOverworldData.currentSaveSlot);
                player.transform.position = player.GetComponent<OverworldStats>().getPlayerPosition();
                player.transform.rotation = StaticOverworldData.playerRotation;
            }
            StaticOverworldData.loadFromMainMenu = false;
        }

        PlayMusic(overworldMusic);
    }

    private void OnDisable()
    {
        player = null;
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

    public GameObject GetPlayer()
    {
        return player;
    }
}
