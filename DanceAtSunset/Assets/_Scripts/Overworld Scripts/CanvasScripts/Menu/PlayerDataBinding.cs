using TMPro;
using UnityEngine;

public enum PlayerDataField
{
    Level,
    Experience,
    ExperienceRequired,
    Gold,
    TotalPlayTime,
    CurrentHealth,
    MaxHealth,
    Attack,
    Defense
}

public class PlayerDataBinding : MonoBehaviour
{
    [SerializeField] private PlayerDataField dataField;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Refresh()
    {
        PlayerData data = PlayerSession.PlayerData;

        switch (dataField)
        {
            case PlayerDataField.Level:
                text.text = data.CurrentLevel.ToString();
                break;

            case PlayerDataField.Experience:
                text.text = data.CurrentExperience.ToString();
                break;

            case PlayerDataField.ExperienceRequired:
                text.text = data.ExperienceRequired.ToString();
                break;

            case PlayerDataField.Gold:
                text.text = data.Gold.ToString();
                break;

            case PlayerDataField.TotalPlayTime:
                text.text = data.TotalPlayTime.ToString();
                break;

            case PlayerDataField.CurrentHealth:
                text.text = data.PlayerBaseStats.CurrentHealth.ToString();
                break;

            case PlayerDataField.MaxHealth:
                text.text = data.PlayerBaseStats.MaxHealth.ToString();
                break;

            case PlayerDataField.Attack:
                text.text = data.PlayerBaseStats.Attack.ToString();
                break;

            case PlayerDataField.Defense:
                text.text = data.PlayerBaseStats.Defense.ToString();
                break;
        }
    }
}