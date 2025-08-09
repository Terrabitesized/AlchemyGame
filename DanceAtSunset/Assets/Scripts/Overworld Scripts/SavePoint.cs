using UnityEngine;
using UnityEngine.UI;

public class SavePoint : MonoBehaviour
{

    [SerializeField] private OverworldStats stats;
    private Canvas displayInteraction;
    private Image saveScreen;
    private Button saveButton1;
    private Button saveButton2;
    private Button saveButton3;

    void Start()
    {
        stats = GameObject.FindWithTag("Player").GetComponent<OverworldStats>();
        displayInteraction = GetComponentInChildren<Canvas>();
        saveScreen = GetComponentInChildren<Image>();
        displayInteraction.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            displayInteraction.enabled = true;
            if (Input.GetKeyDown(KeyCode.E)) {
                stats.SaveToJson();
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            displayInteraction.enabled = false;
        }
    }
}
