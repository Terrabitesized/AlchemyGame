using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AreaTransitionManager : MonoBehaviour
{
    public static AreaTransitionManager Instance;

    private Vector2 transitionDirection;
    private OverworldMovement transitioningPlayerMovement;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 1.5f;

    // :heart_eyes:
    private bool isTransitioning;

    private void Awake()
    {
        Debug.Log((fadeImage == null) ? "fadeImage is null" : "fadeImage is not null");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(FadeIn());
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject fadeObject = GameObject.FindWithTag("FadeBackground");

        if (fadeObject == null)
        {
            Debug.LogWarning("No FadeBackground found in the loaded scene.");
            fadeImage = null;
            return;
        }

        fadeImage = fadeObject.GetComponent<Image>();

        if (fadeImage == null)
        {
            Debug.LogWarning("FadeBackground has no Image component.");
            return;
        }

        
        SetFadeAlpha(1f);
    
    }

    public void TransitionToArea(string sceneName, string spawnPointID)
    {
        if (isTransitioning)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        transitioningPlayerMovement = player != null
            ? player.GetComponent<OverworldMovement>()
            : null;

        if (transitioningPlayerMovement != null)
        {
            // Capture the direction the player was moving upon entering the trigger
            transitionDirection =
                transitioningPlayerMovement.CurrentMovementDirection;

            transitioningPlayerMovement.SetTransitionMovement(
                true,
                transitionDirection
            );
        }

        StartCoroutine(TransitionRoutine(sceneName, spawnPointID));
    }

    private IEnumerator TransitionRoutine(string sceneName, string spawnPointID)
    {
        isTransitioning = true;

        // Fade to black while the player keeps moving
        yield return StartCoroutine(FadeOut());

        // Stop the old player after the fade-out finishes
        if (transitioningPlayerMovement != null)
        {
            transitioningPlayerMovement.SetTransitionMovement(
                false,
                Vector2.zero
            );
        }

        // Always load the destination, even if no movement component was found
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Allow the destination scene to initialize
        yield return null;

        // Position the new player before revealing the scene
        MovePlayerToSpawn(spawnPointID);

        // Fade back in
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private void MovePlayerToSpawn(string spawnPointID)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("AreaTransitionManager could not find the Player.");
            return;
        }

        AreaSpawnPoint[] spawnPoints =
            FindObjectsByType<AreaSpawnPoint>(FindObjectsSortMode.None);

        AreaSpawnPoint destination = null;

        foreach (AreaSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnID == spawnPointID)
            {
                destination = spawnPoint;
                break;
            }
        }

        if (destination == null)
        {
            Debug.LogWarning(
                $"Could not find AreaSpawnPoint with ID '{spawnPointID}'."
            );
            return;
        }

        CharacterController character =
            player.GetComponent<CharacterController>();

        // Disable the CharacterController while teleporting
        if (character != null)
            character.enabled = false;

        player.transform.SetPositionAndRotation(
            destination.transform.position,
            destination.transform.rotation
        );

        // Re-enable the CharacterController
        if (character != null)
            character.enabled = true;

        Debug.Log(
            $"Moved player to '{destination.SpawnID}' at " +
            $"{destination.transform.position}"
        );
    }

    private IEnumerator FadeOut()
    {
        
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float alpha = Mathf.Clamp01(elapsed / fadeOutDuration);

            SetFadeAlpha(alpha);

            yield return null;
        }

        SetFadeAlpha(1f);
    }

    private IEnumerator FadeIn()
    {
        Debug.Log("Fading in...");
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float alpha = 1f - Mathf.Clamp01(elapsed / fadeInDuration);

            SetFadeAlpha(alpha);

            yield return null;
        }

        SetFadeAlpha(0f);
    }



    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null)
            return;

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}