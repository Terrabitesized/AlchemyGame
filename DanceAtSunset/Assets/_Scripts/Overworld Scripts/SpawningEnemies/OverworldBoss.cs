using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverworldBoss : MonoBehaviour
{
    public List<GameObject> Enemies;

    [SerializeReference]
    private List<IDialogueItem> dialogue = new List<IDialogueItem>();

    private OverworldMovement playerMovement;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Freeze player movement and create popup
            playerMovement = other.GetComponent<OverworldMovement>();

            DialogueManager.Instance?.SetDialogue(dialogue);
        }
    }

    private void CombatSetup()
    {
        StaticCombatData.SetupCombat(playerMovement.gameObject, Enemies);
        StaticCombatData.CombatType = CombatType.Boss;

        if (ScreenShatter.Instance != null)
            StartCoroutine(ScreenShatter.Instance.TakeScreenshot());

        SceneManager.LoadScene("CombatTestScene");
    }

    private void CancelBossTrigger()
    {
        playerMovement.gameObject.transform.position = new Vector3(0f, 1f, 0f);

        DialogueManager.Instance?.ToggleDialogueUI(false);
        playerMovement = null;
    }
}
