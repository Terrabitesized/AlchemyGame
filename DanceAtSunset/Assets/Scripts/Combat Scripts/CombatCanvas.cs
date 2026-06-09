using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatCanvas : MonoBehaviour
{
    private CombatManager cm;
    [SerializeField] private GameObject playerHealthbar;
    [SerializeField] private GameObject ingredientText;

    [SerializeField] private GameObject victoryUI;

    private int experienceEarned = 0;
    private int damageDealt = 0;
    private int damageTaken = 0;
    private int ingredientsCollected = 0;
    private int timeTaken = 0;

    [SerializeField] private float returnToOverworldTime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cm = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VictoryCanvas(GameObject p, GameObject c, int exp, int damageD, int damageT, int ing, int time)
    {
        experienceEarned = exp;
        damageDealt = damageD;
        damageTaken = damageT;
        ingredientsCollected = ing;
        timeTaken = time;

        StartCoroutine(VictorySequence(p, c));
    }

    private IEnumerator VictorySequence(GameObject player, GameObject victoryCam)
    {
        // Allow brief period of movement after last enemy has died
        yield return new WaitForSeconds(1f);

        player.GetComponent<PlayerMovement>().canMove = false;
        Debug.Log("Player should not be able to move!");

        // Period to let player register they have won
        yield return new WaitForSeconds(2f);

        // Disable other UI
        playerHealthbar.SetActive(false);
        ingredientText.SetActive(false);

        // Enable Victory UI
        victoryUI.SetActive(true);

        // Destroy old camera, create new one
        Destroy(GameObject.FindGameObjectWithTag("MainCamera"));
        GameObject vCam = Instantiate(victoryCam);
        vCam.transform.parent = player.transform;
        vCam.transform.localPosition = new Vector3(2f, .3f, 4f);
        vCam.transform.localEulerAngles = new Vector3(0f, 180f, 0f);


        // Update Victory UI
        victoryUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("XP Earned: " + experienceEarned);
        victoryUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Damage Dealt: " + damageDealt);
        victoryUI.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText("Damage Taken: " + damageTaken);
        victoryUI.transform.GetChild(5).GetComponent<TextMeshProUGUI>().SetText("Ingredients Collected: " + ingredientsCollected);

        // Calculate time
        int minutes = timeTaken / 60;
        int seconds = timeTaken % 60;

        string min = minutes.ToString();
        string sec = seconds.ToString();
        if(minutes < 10)
        {
            min = "0" + min;
        }
        if (seconds < 10)
        {
            sec = "0" + sec;
        }


        victoryUI.transform.GetChild(6).GetComponent<TextMeshProUGUI>().SetText("Time Taken: " + min + ":" + sec);

        yield return new WaitForSeconds(returnToOverworldTime);

        StaticOverworldData.loadingFromCombat = true;
        SceneManager.LoadScene("CyreneTestScene");

        // Unlock player mouse
        Cursor.lockState = CursorLockMode.None;
        yield return null;
    }

    public void DefeatCanvas(GameObject p)
    {
        StartCoroutine(DefeatSequence(p));
    }

    private IEnumerator DefeatSequence(GameObject player)
    {
        // Allow brief period of movement after last enemy has died
        yield return new WaitForSeconds(1f);



        player.GetComponent<PlayerMovement>().canMove = false;
        Debug.Log("Player should not be able to move!");

        // Period to let player register they have won
        yield return new WaitForSeconds(2f);

        // Disable other UI
        playerHealthbar.SetActive(false);
        ingredientText.SetActive(false);

        // Enable Victory UI
        victoryUI.SetActive(true);

        // Destroy old camera, create new one
        Destroy(GameObject.FindGameObjectWithTag("MainCamera"));
        GameObject vCam = null;
        vCam.transform.parent = player.transform;
        vCam.transform.localPosition = new Vector3(2f, .3f, 4f);
        vCam.transform.localEulerAngles = new Vector3(0f, 180f, 0f);


        // Update Victory UI
        victoryUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("XP Earned: " + experienceEarned);
        victoryUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Damage Dealt: " + damageDealt);
        victoryUI.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText("Damage Taken: " + damageTaken);
        victoryUI.transform.GetChild(5).GetComponent<TextMeshProUGUI>().SetText("Ingredients Collected: " + ingredientsCollected);

        // Calculate time
        int minutes = timeTaken / 60;
        int seconds = timeTaken % 60;

        string min = minutes.ToString();
        string sec = seconds.ToString();
        if (minutes < 10)
        {
            min = "0" + min;
        }
        if (seconds < 10)
        {
            sec = "0" + sec;
        }


        victoryUI.transform.GetChild(6).GetComponent<TextMeshProUGUI>().SetText("Time Taken: " + min + ":" + sec);

        yield return new WaitForSeconds(returnToOverworldTime);

        StaticOverworldData.loadingFromCombat = true;
        SceneManager.LoadScene("CyreneTestScene");
        
        // Unlock player mouse
        Cursor.lockState = CursorLockMode.None;
        yield return null;
    }
}
