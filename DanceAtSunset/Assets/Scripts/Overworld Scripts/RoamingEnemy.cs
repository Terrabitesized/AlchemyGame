using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RoamingEnemy : MonoBehaviour
{
    public Volume v;
    public LensDistortion lens;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        v = v.GetComponent<Volume>();
        v.profile.TryGet(out lens);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CombatSetup();
            //StartCoroutine("TestFunction");
        }
    }

    void CombatSetup()
    {
        StaticCombatData.message = "Balls";
        SceneManager.LoadScene("CombatTestScene");
    }

    private IEnumerator TestFunction()
    {
        Time.timeScale = 0;

        float intVal = 1f;

        while (lens.intensity.value < intVal)
        {
            lens.intensity.value += .1f;
            yield return new WaitForSeconds(.1f);
        }

        lens.intensity.value = 1f;

        yield return new WaitForSeconds(1f);
    }
}
