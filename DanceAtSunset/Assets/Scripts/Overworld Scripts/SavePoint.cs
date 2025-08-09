using UnityEngine;
using UnityEngine.UI;

public class SavePoint : MonoBehaviour
{

    [SerializeField] private OverworldStats stats;
    public Canvas displayInteraction;
    private Image saveScreen;
    private Button saveButton1;
    private Button saveButton2;
    private Button saveButton3;
    private bool paused = false;

    void Start()
    {
        stats = GameObject.FindWithTag("Player").GetComponent<OverworldStats>();
        displayInteraction = GetComponentInChildren<Canvas>();
        saveScreen = GetComponentInChildren<Image>();
        saveScreen.enabled = false;
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
                saveScreen.enabled = true;
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

    private void writeSlot1()
    {

    }
    private void writeSlot2()
    {

    }
    private void writeSlot3()
    {

    }



}
