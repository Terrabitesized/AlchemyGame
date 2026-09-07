using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldBoss : MonoBehaviour
{
    public List<GameObject> enemies;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CombatSetup(other);
        }
    }

    void CombatSetup(Collider player)
    {
        StaticCombatData.SetupCombat(player.gameObject, enemies);
        StaticCombatData.CombatType = CombatType.Boss;

        if (ScreenShatter.Instance != null)
            StartCoroutine(ScreenShatter.Instance.TakeScreenshot());

        SceneManager.LoadScene("CombatTestScene");
    }
}
