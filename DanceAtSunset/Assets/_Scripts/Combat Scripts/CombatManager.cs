using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem;
using Action = System.Action;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public List<GameObject> DEFAULT_ENEMIES = new List<GameObject>();

    [Header("IMPORTANT COMBAT VARIABLES")]
    public readonly int MaxEnemyCount = 3;
    public float ArenaSize = 18f;
    public bool CombatFlow = true; // When false, no inputs can be made and no moves can be cast
    public bool IsBattleOver = false;
    public bool CanCollectCrystals = true;
    public bool ChecekForEnemyDeaths = true;

    public InputHandler InputHandler;

    [Header("Manager References")]
    [SerializeField] private PotionManager pm;
    [SerializeField] private GameObject victoryCam;
    [SerializeField] private GameObject combatBarrier;
    [SerializeField] private TargetingManager targetingManager;
    private bool finalSequencePlaying = false;

    [SerializeField] private GameObject map;

    [Header("Inherited Variables")]
    [SerializeField] private Ultimate ultimate;
    private GameObject cutsceneResources;

    // TOOO CYRENE: MAKE DICTIONARY OF COMBAT SPAWNABLES AS KEY AND PERCENT CHANCE AS VALUES

    [SerializeField] private TextMeshProUGUI subtitles;

    [SerializeField] private GameObject player;
    private Stats playerStats;

    [Header("Dynamic Combat Variables")]
    [SerializeField] private List<CombatCrystal> collectedCrystals = new List<CombatCrystal>();
    [SerializeField] private List<GameObject> enemiesInCombat = new List<GameObject>();
    private List<GameObject> inheritedEnemies;
    private int numberOfCrystals = 0;

    public static int resonanceCharge = -1;
    public static int resonanceChargeMax = 5;

    [Header("Victory Variables")]
    [SerializeField] private int experienceEarned = 0;
    [SerializeField] private int damageDealt = 0;
    [SerializeField] private int damageTaken = 0;
    [SerializeField] private int ingredientsCollected = 0;
    [SerializeField] private int timeTaken = 0;
    private bool wantsToCast;

    // Combat actions
    public static event Action<int> OnCombatStart; // # of enemies present
    public static event Action<bool> OnCombatEnd; // true if win, false if lose

    public static event Action<GameObject> OnEnemyAdded;
    public static event Action<IDamagable> OnEnemyRemoved;

    public static event Action OnSpellCast; // Spell duration
    public static event Action OnSpellFail; // When an invalid spell is cast

    public static event Action OnUltimateCast;
    public static event Action OnCrystalsFired;
    public static event Action OnCrystalsCleared;
    public static event Action OnCrystalsManuallyCleared; // Fires when the player manually clears their ingredients

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        // Set combat flow
        CombatFlow = true;

        // Reset Ultimate charge
        resonanceCharge = StaticCombatData.resonanceCharge;

        // Spawn the player's prefav, will need loaded stats at a later point
        GameObject playPrefab = Instantiate(player);
        playPrefab.transform.position = new Vector3(0f, 1f, -15f);

        player = playPrefab;
        targetingManager = player.GetComponentInChildren<TargetingManager>();

        // Enable combat input
        InputHandler?.EnableCombatInput();

        // Check for tutorial status
        if (StaticCombatData.TutorialType != Tutorial.None)
            TutorialManager.SetupTutorial(InputHandler);

        // Load data from StaticCombatData
        if(StaticCombatData.enemies != null && StaticCombatData.enemies.Count > 0)
            inheritedEnemies = new List<GameObject>(StaticCombatData.enemies);
        else
            inheritedEnemies = new List<GameObject>(DEFAULT_ENEMIES);

        // Set arena size based on CombatType
        switch (StaticCombatData.CombatType)
        {
            case CombatType.Normal:
                ArenaSize = 18f;
                break;
            case CombatType.Boss:
                ArenaSize = 25f;
                break;
        }

        // Pre-load any ultimate related effects
        if (ultimate.HideMap)
        {
            GameObject ultMap = Instantiate(ultimate.CutsceneMap);
            ultMap.transform.position = Vector3.zero;
            ultMap.SetActive(false);
        }


        // TODO CYRENE: This is a FUCKASS way of doing this plz fix when you can be
        // bothered to find a better solution
        cutsceneResources = Instantiate(ultimate.CutsceneResources);
        cutsceneResources.transform.position = Vector3.zero;
        cutsceneResources.SetActive(false);

        ultimate.SetCutsceneAnimator(cutsceneResources.GetComponentInChildren<Animator>());

        // Set size of combat barrier
        float size = ArenaSize / 18f;
        combatBarrier.transform.localScale = Vector3.one * size;

        // Lock player mouse
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        DamageCrystal.OnCrystalCollected += AddCrystal;

        PlayerStats.OnPlayerDied += OnPlayerDied;

        InputHandler.PlayerInput.Combat.Cast.performed += CastCurrentSpell;
        InputHandler.PlayerInput.Combat.Ultimate.performed += CastUltimateAbility;
        InputHandler.PlayerInput.Combat.ClearIngredients.performed += ClearCrystals;
    }

    private void OnDisable()
    {
        DamageCrystal.OnCrystalCollected -= AddCrystal;

        PlayerStats.OnPlayerDied -= OnPlayerDied;

        InputHandler.PlayerInput.Combat.Cast.performed -= CastCurrentSpell;
        InputHandler.PlayerInput.Combat.Ultimate.performed -= CastUltimateAbility;
        InputHandler.PlayerInput.Combat.ClearIngredients.performed -= ClearCrystals;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        // Set reference to PlayerStats
        playerStats = player.GetComponent<PlayerStats>().Stats;

        // Start counting battle duration
        StartCoroutine(increaseTimeTaken());

        subtitles = CombatCanvas.Instance?.GetSubtitles();

        // Check to make sure enemy data was properly loaded
        if (inheritedEnemies.Count == 0)
            IsBattleOver = true;
        else
        {
            // Spawn enemies based on what enemies were in spawned roamable in overworld
            if (inheritedEnemies.Count > 0)
            {
                // Spawn the only enemy in center
                if (inheritedEnemies.Count == 1)
                {
                    GameObject temp = Instantiate(inheritedEnemies[0]);
                    temp.GetComponent<BehaviorGraphAgent>().SetVariableValue<GameObject>("Self", temp);
                    temp.transform.position = new Vector3(0f, 1f, 0f);

                    // Reassign reference to clone, as to not modify prefab
                    SetEnemyName(temp, inheritedEnemies[0].name);
                    enemiesInCombat.Add(temp);
                    OnEnemyAdded?.Invoke(temp);
                }
                else
                {
                    // Spawn enemies slightly spread apart
                    for (int i = 0; i < inheritedEnemies.Count; i++)
                    {
                        GameObject temp = Instantiate(inheritedEnemies[i]);
                        temp.name = inheritedEnemies[i].name;
                        temp.GetComponent<BehaviorGraphAgent>().SetVariableValue<GameObject>("Self", temp);
                        float x_Pos = UnityEngine.Random.Range(-5f, 5f);
                        float z_Pos = UnityEngine.Random.Range(-5f, 5f);
                        temp.transform.position = new Vector3(x_Pos, 1f, z_Pos);

                        // Reassign reference to clone, as to not modify prefab
                        SetEnemyName(temp, inheritedEnemies[i].name);
                        enemiesInCombat.Add(temp);
                        OnEnemyAdded?.Invoke(temp);
                    }
                }
            }
            else
            {
                // End combat
                IsBattleOver = true;
            }

            // Set up Potion Manager
            //pm.SetupPM(player, enemiesInCombat);

            // Invoke combat start event
            OnCombatStart?.Invoke(enemiesInCombat.Count);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsBattleOver)
        {
            // Allows player to dispense their collected ingredients
            //if (Input.GetKey(KeyCode.Space))
            //    Time.timeScale = 0f;
            //else
            //    Time.timeScale = 1f;

            // Checks if game should end
            if (playerStats.CurrentHealth <= 0)
            {
                Debug.Log("Player has died! Game should end");

            }
            if (enemiesInCombat.Count == 0)
            {
                Debug.Log("All enemies have died! Game should end");
                IsBattleOver = true;

                // Save resonanceCharge for next fight
                StaticCombatData.resonanceCharge = resonanceCharge;

                StaticCombatData.experienceEarned = experienceEarned;

                if(StaticCombatData.BaseStats != null)
                    StaticCombatData.BaseStats.currentHealth = playerStats.CurrentHealth;

                if (!finalSequencePlaying)
                {
                    OnCombatEnd?.Invoke(true);

                    CombatCanvas.Instance?.VictoryCanvas(player, victoryCam,
                        experienceEarned, damageDealt, damageTaken, ingredientsCollected,
                        timeTaken);

                    finalSequencePlaying = true;
                }
            }
        }
    }

    public void OnPlayerDied()
    {
        IsBattleOver = true;

        if (!finalSequencePlaying)
        {
            OnCombatEnd?.Invoke(false);

            CombatCanvas.Instance?.DefeatCanvas(player);

            finalSequencePlaying = true;
        }
    }

    public void AddCrystal(CombatCrystal crystal)
    {
        if (!CanCollectCrystals)
            return;

        collectedCrystals.Add(crystal);

        // Up victory tally
        ingredientsCollected++;

        // Increase Resonance Charge
        if (resonanceCharge < resonanceChargeMax)
            resonanceCharge++;

        string hexColor = "#" + ColorUtility.ToHtmlStringRGB(crystal.color);

        if (numberOfCrystals == 0)
        {
            subtitles.SetText($"<color={hexColor}>" + crystal.crystalSpell.spellName + "</color>");
        }
        else if (numberOfCrystals > 0)
        {
            subtitles.SetText(subtitles.text + " + " + $"<color={hexColor}>" + crystal.crystalSpell.spellName + "</color>");
        }

        numberOfCrystals++;

        if (numberOfCrystals == 3)
        {
            wantsToCast = true;
            StartCoroutine(CastAllSpells());
        }

    }

    private IEnumerator CastAllSpells()
    {
        subtitles.SetText("Empty");


        yield return new WaitForEndOfFrame();
        // CAST ALL SPELLS
        OnSpellCast?.Invoke();

        ClearCrystals();
        OnCrystalsCleared?.Invoke();
    }

    private void CastCurrentSpell(InputAction.CallbackContext context)
    {
        if (!wantsToCast || pm.autoCasting)
            return;

        pm.CastCurrentSpell();
    }

    private void ClearCrystals()
    {
        collectedCrystals.Clear();

        numberOfCrystals = 0;
    }

    private void ClearCrystals(InputAction.CallbackContext context)
    {
        OnCrystalsManuallyCleared?.Invoke();
        ClearCrystals();
    }

    private void CastUltimateAbility(InputAction.CallbackContext context)
    {
        if (resonanceCharge >= resonanceChargeMax)
        {
            resonanceCharge = 0;
            StartCoroutine(CastUltimateCoroutine());
        }
    }

    private IEnumerator CastUltimateCoroutine()
    {
        Debug.Log("CAST ULTIMATE");
        OnUltimateCast?.Invoke();

        CombatFlow = false;

        yield return new WaitForSeconds(1f);

        CombatFlow = true;

        yield return StartCoroutine(UltimateManager.Instance.PlayUltimate(ultimate, this));
    }

    private void SetEnemyName(GameObject newEnemy, string originalName)
    {
        EnemyStats newStats = newEnemy.GetComponent<EnemyStats>();
        if (newStats == null)
            return;

        // Find other spawned enemies of the same type.
        List<GameObject> sameTypeEnemies = new List<GameObject>();

        foreach (GameObject enemy in enemiesInCombat)
        {
            if (enemy == null || enemy == newEnemy)
                continue;

            EnemyStats existingStats = enemy.GetComponent<EnemyStats>();

            if (existingStats != null &&
                existingStats.EnemyType == newStats.EnemyType)
            {
                sameTypeEnemies.Add(enemy);
            }
        }

        // The first enemy of this type keeps the original name.
        if (sameTypeEnemies.Count == 0)
        {
            newEnemy.name = originalName;
            return;
        }

        // Rename the first unsuffixed enemy to A.
        bool hasSuffix = false;

        foreach (GameObject enemy in sameTypeEnemies)
        {
            if (enemy.name == originalName)
            {
                enemy.name = $"{originalName} A";
                hasSuffix = true;
                break;
            }
        }

        // Find the next available suffix.
        int nextLetter = 0;

        foreach (GameObject enemy in sameTypeEnemies)
        {
            string enemyName = enemy.name;

            if (!enemyName.StartsWith(originalName + " "))
                continue;

            string suffix = enemyName.Substring(originalName.Length + 1);

            if (suffix.Length == 1 &&
                suffix[0] >= 'A' && suffix[0] <= 'Z')
            {
                nextLetter = Mathf.Max(nextLetter, suffix[0] - 'A' + 1);
            }
        }

        // If there were existing enemies but none had a suffix,
        // the new enemy should be B after the first becomes A.
        if (!hasSuffix && nextLetter == 0)
            nextLetter = 1;

        // A-Z suffixes only.
        if (nextLetter >= 26)
        {
            Debug.LogWarning($"Too many enemies named {originalName} to assign a unique A-Z suffix.");
            newEnemy.name = $"{originalName} {nextLetter + 1}";
            return;
        }

        newEnemy.name = $"{originalName} {(char)('A' + nextLetter)}";
    }

    public void AddEnemy(GameObject enemy)
    {
        Debug.Log("I have been passed " + enemy.name + " to add!");

        GameObject newEnemy = Instantiate(enemy);
        newEnemy.GetComponent<BehaviorGraphAgent>().SetVariableValue<GameObject>("Self", newEnemy);
        float x_Pos = UnityEngine.Random.Range(-10f, 10f);
        float z_Pos = UnityEngine.Random.Range(-10f, 10f);
        newEnemy.transform.position = new Vector3(x_Pos, 1f, z_Pos);

        SetEnemyName(newEnemy, enemy.name);
        enemiesInCombat.Add(newEnemy);
        OnEnemyAdded?.Invoke(newEnemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        Debug.Log("I have been passed " + enemy.name + " to remove!");

        // Grants experience based on enemy level disparity
        //float levelMod = ((enemy.GetComponent<EnemyStats>().getLevel() - player.GetComponent<PlayerStats>().getLevel()) * .05f) + 1;
        int trueExp = Mathf.FloorToInt(enemy.GetComponent<EnemyStats>().GetExperience());
        if (trueExp > 0)
        {
            experienceEarned += trueExp;
            Debug.Log("You earned " + trueExp + " experience!");
        } else
        {
            experienceEarned += 1;
            Debug.Log("You earned " + 1 + " experience!");
        }

        // If the enemy had any popups parented to them, we need to unparent and move them
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (child.CompareTag("CombatPopup"))
            {
                child.transform.parent = null;
                child.transform.position = new Vector3(0f, -100f, 0f);
            }
        }

        enemiesInCombat.Remove(enemy);
        OnEnemyRemoved?.Invoke(enemy.GetComponent<IDamagable>());
        Destroy(enemy);
    }

    public int CalculateDamage(Stats attacker, Stats defender, int basePower)
    {
        // If attackerStats == null, this is a DoT or True Damage effect
        if (attacker == null)
            return basePower;

        float attackerAttack = attacker.Attack;
        float defenderDefense = defender.Defense;

        return Mathf.RoundToInt((Mathf.Pow(attackerAttack, 1.2f) / (defenderDefense + 10f)) * basePower * 1) + 1;
    }

    public void ToggleMapVisibility(bool val)
    {
        foreach (MeshRenderer mr in map.GetComponentsInChildren<MeshRenderer>())
            mr.enabled = val;
    }

    public void ToggleArenaBarrierVisibility(bool val)
    {
        combatBarrier.SetActive(val);
    }

    public void TogglePlayerVisibility(bool val)
    {
        foreach (MeshRenderer m in player.GetComponentsInChildren<MeshRenderer>())
        {
            m.enabled = val;
        }
    }

    public void ToggleCutsceneResources(bool val)
    {
        cutsceneResources.SetActive(val);
    }

    public int GetCollectedCrystalsCount()
    {
        return collectedCrystals.Count;
    }

    public GameObject GetPlayerGameObject()
    {
        return player;
    }

    public IDamagable GetPlayerDamagable()
    {
        return player.GetComponent<IDamagable>();
    }

    public TargetingManager GetTargetingManager()
    {
        return targetingManager;
    }

    public List<IDamagable> GetEnemiesInCombat()
    {
        List<IDamagable> enemies = new List<IDamagable>();

        foreach(GameObject enemy in enemiesInCombat)
            enemies.Add(enemy.GetComponent<IDamagable>());

        return enemies;
    }

    public GameObject GetNearestEnemy()
    {
        if(enemiesInCombat.Count == 1)
            return enemiesInCombat[0];

        int closestEnemyIndex = 0;

        for (int i = 1; i < enemiesInCombat.Count; i++)
        {
            if(Vector3.Distance(player.transform.position, enemiesInCombat[i].transform.position) <
                Vector3.Distance(player.transform.position, enemiesInCombat[closestEnemyIndex].transform.position))
            {
                closestEnemyIndex = i;
            }
        }

        return enemiesInCombat[closestEnemyIndex];
    }

    public int GetEnemyCount()
    {
        return enemiesInCombat.Count;
    }

    // VICTORY STAT SETTERS

    public void IncreaseDamageDealt(int damage)
    {
        damageDealt += damage;
    }

    public void IncreaseDamageTaken(int damage)
    {
        damageTaken += damage;
    }

    private IEnumerator increaseTimeTaken()
    {
        while(!IsBattleOver)
        {
            yield return GameFlowUtility.WaitForGameplaySeconds(1f);
            timeTaken++;
        }
    }
}

public static class GameFlowUtility
{
    public static IEnumerator WaitForGameplaySeconds(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (CombatManager.Instance != null && CombatManager.Instance.CombatFlow)
            {
                elapsed += Time.deltaTime;
            }

            yield return null;
        }
    }
}
