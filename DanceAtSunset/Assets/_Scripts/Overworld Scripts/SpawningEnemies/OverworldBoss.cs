using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverworldBoss : MonoBehaviour
{
    public List<GameObject> enemies;
    private OverworldMovement playerMovement;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Freeze player movement and create popup
            playerMovement = other.GetComponent<OverworldMovement>();

            // Disable movement and enable UI
            playerMovement.GetInputHandler().EnableUIInput();

            DialogueCanvas.Instance?.ToggleDialogueUI(true);

            List<Button> buttons = DialogueCanvas.Instance?.GetDialogueButtons();

            if (buttons == null)
                return;

            buttons[0].onClick.AddListener(CombatSetup);
            buttons[1].onClick.AddListener(CancelBossTrigger);
        }
    }

    private void CombatSetup()
    {
        StaticCombatData.SetupCombat(playerMovement.gameObject, enemies);
        StaticCombatData.CombatType = CombatType.Boss;

        if (ScreenShatter.Instance != null)
            StartCoroutine(ScreenShatter.Instance.TakeScreenshot());

        SceneManager.LoadScene("CombatTestScene");
    }

    private void CancelBossTrigger()
    {
        playerMovement.gameObject.transform.position = new Vector3(0f, 1f, 0f);

        playerMovement.GetInputHandler().EnableOverworldInput();

        DialogueCanvas.Instance?.ToggleDialogueUI(false);
        playerMovement = null;
    }
}
