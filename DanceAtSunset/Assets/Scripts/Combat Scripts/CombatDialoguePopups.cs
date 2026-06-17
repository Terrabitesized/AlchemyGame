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
        PotionManager.OnAttackEnd += CheckForDefeatedEnemies;
        CombatManager.OnCombatEnd += CheckForBattleEnd;
    }

    private void OnDisable()
    {
        PotionManager.OnAttackEnd -= CheckForDefeatedEnemies;
        CombatManager.OnCombatEnd -= CheckForBattleEnd;
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

            message += deadEnemyCount + " of them died.";
            popupHolder.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "That's one enemy down!";

            StartCoroutine(FadeDialoguePopup(0f));
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
            popupHolder.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "The enemy is getting stronger too.";
        else
            popupHolder.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Get up! We need you!";

        StartCoroutine(FadeDialoguePopup(0f));
    }

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
}
