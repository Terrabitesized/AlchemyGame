using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavePoint : MonoBehaviour
{
    [Header("Profile")]
    public OverworldStats data;
    [SerializeField] private int profile = 0; // file to be loaded from

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;

    [SerializeField] private TextMeshProUGUI levelText;

    void Start()
    {
        data = GameObject.FindWithTag("Player").GetComponent<OverworldStats>();
        checkData();
    }

    private void checkData()
    {
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            levelText.text = "Level: " + data.getLevel();
        }
    }


}
