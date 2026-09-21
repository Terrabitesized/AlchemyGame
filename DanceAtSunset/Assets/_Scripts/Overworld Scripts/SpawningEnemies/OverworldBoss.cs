using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Burst.Intrinsics.X86.Avx;

public class OverworldBoss : MonoBehaviour
{
    public List<GameObject> Enemies;

    [SerializeField] private DialogueData dialogueData;

    [Header("Boss Intro Animation Variables")]
    [SerializeField] private CinemachineCamera animationCamera;
    [SerializeField] private Vector3 playerStartPosition;
    [SerializeField] private Vector3 playerEndPosition;
    [SerializeField] private float introAnimationDuration;

    private OverworldMovement playerMovement;
    private bool inDialogue = false;
    private bool inAnimation = false;

    private void OnDisable()
    {
        if(inDialogue)
        {
            FireEventAction.OnFireEvent -= PlayBossAnimation;
            EndDialogueAction.OnDialogueEnded -= DialogueEnd;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (inAnimation)
            return;

        if (other.tag == "Player")
        {
            // Freeze player movement and create popup
            playerMovement = other.GetComponent<OverworldMovement>();

            DialogueManager.Instance?.SetDialogue(dialogueData.dialogue);

            // Subcribe to events
            FireEventAction.OnFireEvent += PlayBossAnimation;
            EndDialogueAction.OnDialogueEnded += DialogueEnd;

            inDialogue = true;
        }
    }

    private void PlayBossAnimation() { StartCoroutine(PlayBossAnimationCoroutine()); }

    private IEnumerator PlayBossAnimationCoroutine()
    {
        inAnimation = true;
        OverworldEnemySpawning.Instance?.DespawnAllEnemies(false);

        playerMovement.GetInputHandler().DisableAllInput();
        animationCamera.Priority = 3;

        GameObject player = playerMovement.gameObject;

        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / introAnimationDuration;

            // Move player
            player.transform.position = Vector3.Lerp(playerStartPosition, playerEndPosition, progress);
        }

        CombatSetup();
    }

    private void DialogueEnd()
    {
        inDialogue = false;

        FireEventAction.OnFireEvent -= PlayBossAnimation;
        EndDialogueAction.OnDialogueEnded -= DialogueEnd;
    }

    private void CombatSetup()
    {
        StaticCombatData.SetupCombat(playerMovement.gameObject, Enemies, CombatType.Boss);

        if (ScreenShatter.Instance != null)
            StartCoroutine(ScreenShatter.Instance.TakeScreenshot());

        SceneManager.LoadScene("CombatTestScene");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerStartPosition, 1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerEndPosition, 1f);
    }
}
