using TMPro;
using UnityEngine;

public class OverworldDebugConsole : MonoBehaviour
{
    private bool isEnabled = false;
    private CanvasGroup debugCanvas;

    [Header("Player Stats")]
    [SerializeField] private TextMeshProUGUI currentHealth;
    [SerializeField] private TextMeshProUGUI maxHealth;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI exp;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI gold;

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

        if (PlayerSession.PlayerData == null)
            return;

        currentHealth.text = $"Current Health: {PlayerSession.PlayerData.PlayerBaseStats.CurrentHealth.ToString()}";
        maxHealth.text = $"Max Health: {PlayerSession.PlayerData.PlayerBaseStats.MaxHealth.ToString()}";
        attack.text = $"Attack: {PlayerSession.PlayerData.PlayerBaseStats.Attack.ToString()}";
        defense.text = $"Defense: {PlayerSession.PlayerData.PlayerBaseStats.Defense.ToString()}";
        exp.text = $"Experience: {PlayerSession.PlayerData.CurrentExperience.ToString()} / " +
            $"{PlayerSession.PlayerData.ExperienceRequired.ToString()}";
        level.text = $"Level: {PlayerSession.PlayerData.CurrentLevel.ToString()}";
        gold.text = $"Gold: {PlayerSession.PlayerData.Gold.ToString()}";
    }
}
