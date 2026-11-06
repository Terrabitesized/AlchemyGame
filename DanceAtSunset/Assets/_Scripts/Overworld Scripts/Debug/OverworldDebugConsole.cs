using TMPro;
using UnityEngine;

public class OverworldDebugConsole : MonoBehaviour
{
    private BaseStats playerStats;
    private bool isEnabled = false;
    private CanvasGroup debugCanvas;

    [Header("Player Stats")]
    [SerializeField] private TextMeshProUGUI currentHealth;
    [SerializeField] private TextMeshProUGUI maxHealth;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI exp;
    [SerializeField] private TextMeshProUGUI level;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        debugCanvas = GetComponentInChildren<CanvasGroup>();
        debugCanvas.alpha = 0f;
        isEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            isEnabled = !isEnabled;
            debugCanvas.alpha = isEnabled ? 1f : 0f;
        }

        if (!isEnabled)
            return;

        if (playerStats == null)
            playerStats = OverworldPlayerStats.Instance?.stats;

        currentHealth.text = $"Current Health: {playerStats.currentHealth.ToString()}";
        maxHealth.text = $"Max Health: {playerStats.maxHealth.ToString()}";
        attack.text = $"Attack: {playerStats.attack.ToString()}";
        defense.text = $"Defense: {playerStats.defense.ToString()}";
    }
}
