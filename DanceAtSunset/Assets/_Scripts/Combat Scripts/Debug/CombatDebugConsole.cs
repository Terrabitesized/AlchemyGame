using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CombatDebugConsole : MonoBehaviour
{
    public InputHandler InputHandler;

    [Header("Enemy Bars")]
    [SerializeField] private Transform enemyDebugBarsHolder;
    [SerializeField] private GameObject enemyDebugBar;
    private Dictionary<IDamagable, GameObject> enemyDebugBarMap = new Dictionary<IDamagable, GameObject>();

    [Header("Stat Viewer")]
    [SerializeField] private GameObject statViewer;
    private StatGroupHolder statGroupHolder;
    private IDamagable currentAnalyzedEnemy = null;
    private Coroutine analyzeEnemyCoroutine;

    private CanvasGroup debugConsole;
    private bool debugConsoleEnabled;

    private void Awake()
    {
        debugConsole = GetComponent<CanvasGroup>();
        debugConsole.alpha = 0f;
        //debugConsole.blocksRaycasts = false;
        debugConsole.interactable = false;

        debugConsoleEnabled = false;
        statGroupHolder = statViewer.GetComponent<StatGroupHolder>();
    }

    private void OnEnable()
    {
        InputHandler.PlayerInput.Combat.DebugMenu.started += ToggleDebugConsole;

        CombatManager.OnEnemyAdded += AddEnemy;
        CombatManager.OnEnemyRemoved += RemoveEnemy;
    }

    private void OnDisable()
    {
        InputHandler.PlayerInput.Combat.DebugMenu.started -= ToggleDebugConsole;

        CombatManager.OnEnemyAdded -= AddEnemy;
        CombatManager.OnEnemyRemoved -= RemoveEnemy;
    }

    private void ToggleDebugConsole(InputAction.CallbackContext context)
    {
        debugConsoleEnabled = !debugConsoleEnabled;

        debugConsole.alpha = debugConsoleEnabled ? 1f : 0f;
        debugConsole.interactable = debugConsoleEnabled;

        Cursor.lockState = debugConsoleEnabled ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void AddEnemy(GameObject enemy)
    {
        GameObject temp = Instantiate(enemyDebugBar);
        temp.transform.SetParent(enemyDebugBarsHolder);

        IDamagable damagable = enemy.GetComponent<IDamagable>();

        enemyDebugBarMap.TryAdd(damagable, temp);

        // Populate bar
        temp.GetComponent<EnemyBarButtonHolder>().EnemyName.text = enemy.name;
        List<Button> buttons = temp.GetComponent<EnemyBarButtonHolder>().BarButtons;

        if (buttons != null)
        {
            buttons[0].onClick.AddListener(() => damagable.TakeDamage(-9999, null));
            buttons[1].onClick.AddListener(() => damagable.TakeDamage(9999, null));
            buttons[2].onClick.AddListener(() =>
            {
                enemy.GetComponent<BehaviorGraphAgent>().enabled = !enemy.GetComponent<BehaviorGraphAgent>().enabled;
            });
            buttons[3].onClick.AddListener(() => PopulateStatsMenu(enemy));
        }
    }

    private void RemoveEnemy(IDamagable enemy)
    {
        enemyDebugBarMap.TryGetValue(enemy, out GameObject bar);
        enemyDebugBarMap.Remove(enemy);

        if (bar != null)
            Destroy(bar);
    }

    private void PopulateStatsMenu(GameObject enemy)
    {
        if(!statViewer.activeSelf)
            statViewer.SetActive(true);

        currentAnalyzedEnemy = enemy.GetComponent<IDamagable>();

        if(analyzeEnemyCoroutine != null)
            StopCoroutine(analyzeEnemyCoroutine);

        analyzeEnemyCoroutine = StartCoroutine(UpdateStatsMenu(enemy));
    }

    private IEnumerator UpdateStatsMenu(GameObject enemy)
    {
        statGroupHolder.EnemyName.text = enemy.name;

        while(true)
        {
            // Check if we need to clean up
            if(!enemyDebugBarMap.TryGetValue(currentAnalyzedEnemy, out GameObject v))
            {
                statViewer.SetActive(false);
                analyzeEnemyCoroutine = null;
                yield break;
            }

            statGroupHolder.EnemyHealth.text = $"HP : {currentAnalyzedEnemy.Stats.CurrentHealth} / " +
                $"{currentAnalyzedEnemy.Stats.MaxHealth}";

            statGroupHolder.EnemyShield.text = $"Shield : {currentAnalyzedEnemy.Stats.Shield}";
            statGroupHolder.EnemyAttack.text = $"Attack : {currentAnalyzedEnemy.Stats.Attack}";
            statGroupHolder.EnemyDefence.text = $"Defence : {currentAnalyzedEnemy.Stats.Defense}";

            yield return new WaitForSeconds(.5f);
        }
    }
}
