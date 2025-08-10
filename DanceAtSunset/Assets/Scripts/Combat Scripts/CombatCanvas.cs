using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatCanvas : MonoBehaviour
{
    [SerializeField] private GameObject playerHealthbar;
    [SerializeField] private GameObject ingredientText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VictoryCanvas(GameObject p, GameObject c)
    {
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

        // Destroy old camera, create new one
        Destroy(GameObject.FindGameObjectWithTag("MainCamera"));
        GameObject vCam = Instantiate(victoryCam);
        vCam.transform.parent = player.transform;
        vCam.transform.localPosition = new Vector3(2f, .3f, 4f);
        vCam.transform.localEulerAngles = new Vector3(0f, 180f, 0f);


        yield return new WaitForSeconds(.25f);

        //canvas.GetComponent<CombatCanvas>().VictoryCanvas();

        yield return null;

        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("NateTestScene");
        yield return null;
    }
}
