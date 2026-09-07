using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class CombatDialoguePopups : MonoBehaviour
{
    [SerializeField] private GameObject popupHolder;

    private void OnEnable()
    {
        CombatManager.OnCombatStart += OnBattleStart;
        CombatManager.OnCombatEnd += CheckForBattleEnd;
    }

    private void OnDisable()
    {
        CombatManager.OnCombatStart -= OnBattleStart;
        CombatManager.OnCombatEnd -= CheckForBattleEnd;
    }

    private void OnBattleStart(int enemiesInCombatCount)
    {
        switch(enemiesInCombatCount)
        {
            case 0:
                SetDialoguePopupText("Huh? There's no enemies?");
                break;
            case 1:
                SetDialoguePopupText("One enemy! And it's all alone");
                break;
            case 2:
                SetDialoguePopupText("Be careful, there's 2 enemies!");
                break;
            case 3:
                SetDialoguePopupText("Look out! There's 3 of them.");
                break;
        }

        StartCoroutine(FadeDialoguePopup(.5f));
    }

    private void CheckForDefeatedEnemies(List<GameObject> enemies, List<bool> enemiesAlive)
    {
        Debug.Log("I THINK SOMEONE FUCKING DIED AHHHHHHHHHHHHHHHHHHHHH" + enemies.Count);

        if (popupHolder != null)
        {
            // Check how many enemies were defeated
            string message = "You hit " + enemies.Count + " enemies and ";
            int deadEnemyCount = 0;
            foreach(bool status in enemiesAlive)
            {
                if(!status)
                    deadEnemyCount++;
            }

            switch(deadEnemyCount)
            {
                case 0:

                    break;

                case 1:
                    message += deadEnemyCount + " of them died.";
                    SetDialoguePopupText("That's one enemy down!");

                    StartCoroutine(FadeDialoguePopup(0f));
                    break;
                case 2:
                    message += deadEnemyCount + " of them died.";
                    SetDialoguePopupText("Two at once! Keep it up.");

                    StartCoroutine(FadeDialoguePopup(0f));
                    break;
            }
        }
    }

    private void CheckForBattleEnd(bool val)
    {
        StartCoroutine(CheckForBattleEndCoroutine(val));
    }

    private IEnumerator CheckForBattleEndCoroutine(bool val)
    {
        yield return new WaitForSeconds(3f);

        if (val)
            SetDialoguePopupText("The enemy is getting stronger too.");
        else
            SetDialoguePopupText("Get up! We need you!");

        StartCoroutine(FadeDialoguePopup(0f));
    }

    // HELPER METHODS

    private IEnumerator FadeDialoguePopup(float delay)
    {
        yield return new WaitForSeconds(delay);

        CanvasGroup cg = popupHolder.GetComponent<CanvasGroup>();

        for(float i = 0; i < 1f; i += .05f)
        {
            cg.alpha = i;
            yield return new WaitForSeconds(.02f);
        }

        cg.alpha = 1f;

        yield return new WaitForSeconds(2f);

        for (float i = 1f; i > 0f; i -= .05f)
        {
            cg.alpha = i;
            yield return new WaitForSeconds(.02f);
        }

        cg. alpha = 0f;
    }

    private void SetDialoguePopupText(string text)
    {
        popupHolder.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }
}
