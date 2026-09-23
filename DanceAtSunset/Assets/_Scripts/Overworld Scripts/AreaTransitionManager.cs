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
        // Make sure the screen starts fully visible.
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
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Give the new scene a frame to finish initializing.
        yield return null;

        // Find the destination spawn point.
        AreaSpawnPoint[] spawnPoints =
            FindObjectsByType<AreaSpawnPoint>(FindObjectsSortMode.None);

        foreach (AreaSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnID == spawnPointID)
            {
                MovePlayerToSpawn(spawnPoint);
                break;
            }
        }

        // Fade back into the new area.
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private void MovePlayerToSpawn(AreaSpawnPoint spawnPoint)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("AreaTransitionManager could not find the Player.");
            return;
        }

        player.transform.SetPositionAndRotation(
            spawnPoint.transform.position,
            spawnPoint.transform.rotation
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