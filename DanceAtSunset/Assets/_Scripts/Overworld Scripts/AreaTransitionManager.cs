using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AreaTransitionManager : MonoBehaviour
{
    public static AreaTransitionManager Instance;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.5f;

    // :heart_eyes:
    private bool isTransitioning;

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

    private void Start()
    {
        SetFadeAlpha(0f);
    }

    public void TransitionToArea(string sceneName, string spawnPointID)
    {
        if (isTransitioning)
            return;

        StartCoroutine(TransitionRoutine(sceneName, spawnPointID));
    }

    private IEnumerator TransitionRoutine(string sceneName, string spawnPointID)
    {
        isTransitioning = true;

        // Fade to black while the player continues moving.
        yield return StartCoroutine(FadeOut());

        // Load the destination scene.
        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Let the destination scene finish its Start/Awake initialization.
        yield return null;

        // Find and position the new scene's player.
        MovePlayerToSpawn(spawnPointID);

        // Fade back in.
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

        // Disable the CharacterController while teleporting.
        if (character != null)
            character.enabled = false;

        player.transform.SetPositionAndRotation(
            destination.transform.position,
            destination.transform.rotation
        );

        // Re-enable the CharacterController.
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