using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticCombatTrigger : MonoBehaviour
{
    [Header("Combat Parameters")]
    public List<GameObject> Enemies;
    public Tutorial TutorialType;
    public List<string> SceneNames = new List<string>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CombatSetup(other);
        }
    }

    private void CombatSetup(Collider player)
    {
        StaticCombatData.SetupCombat(player.gameObject.transform, Enemies, CombatType.Boss);

        if (ScreenShatter.Instance != null)
            StartCoroutine(ScreenShatter.Instance.TakeScreenshot());

        if (SceneNames.Count == 0)
            SceneManager.LoadScene(OverworldManager.Instance.CurrentZoneCombatSceneName);
        else
        {
            SceneManager.LoadScene(SceneNames[Random.Range(0, SceneNames.Count)]);
        }
    }

    private void OnValidate()
    {
        if (Enemies.Count == 0)
            Debug.LogWarning($"{gameObject.name} has no enemies in StaticCombatTrigger!");
    }
}
