using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatCanvas : MonoBehaviour
{
    public static CombatCanvas Instance;
    [SerializeField] private CanvasGroup combatCanvasGroup;

    [Header("Health Bar UI")]
    [SerializeField] private GameObject healthBarHolder;
    [SerializeField] private TextMeshProUGUI currentHealthText;
    [SerializeField] private TextMeshProUGUI maxHealthText;
    [SerializeField] private Image healthbarImage;
    private readonly int MASK_PERCENT = Shader.PropertyToID("_MaskPercent");
    [SerializeField] private AnimationCurve damageSliderAnimCurve;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider damageSlider;
    [SerializeField] private float damageSliderAnimateTime = .5f;
    private Coroutine damageBarAnimateCoroutine;
    [SerializeField] private List<GameObject> healthBarDecor = new List<GameObject>();
    [SerializeField] private List<float> decorPercentages = new List<float>();
    private Coroutine[] decorCoroutines;
    private bool[] decorStates;


    [Header("Ultimate Bar UI")]
    [SerializeField] private GameObject ultimateBarHolder;
    [SerializeField] private TextMeshProUGUI ultimateText;
    [SerializeField] private Slider ultimateSlider;
    [SerializeField] private AnimationCurve ultimateSliderAnimCurve;
    [SerializeField] private float ultimateSliderAnimateTime = .5f;
    private Coroutine ultimateBarAnimateCoroutine;

    [Header("Popup Bar")]
    [SerializeField] private CanvasGroup popupCanvasGroup;
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private AnimationCurve popupAlphaAnimCurve;
    [SerializeField] private float popupAlphaAnimateTime = .5f;
    private Coroutine popupAlphaAnimateCoroutine;

    [Header("Spell UI")]
    [SerializeField] private GameObject crystalsCollectedText;
    [SerializeField] private GameObject ingredientText;

    [SerializeField] private GameObject victoryUI;

    private int experienceEarned = 0;
    private int damageDealt = 0;
    private int damageTaken = 0;
    private int ingredientsCollected = 0;
    private int timeTaken = 0;

    [SerializeField] private float returnToOverworldTime = 5f;
    private CombatManager combatManager;

    private void OnEnable()
    {
        PlayerStats.OnPlayerDamaged += UpdateHealthBar;
        DamageCrystal.OnCrystalCollected += UpdateUltimateBar;
        CombatManager.OnUltimateCast += UpdateUltimateBar;
        CombatManager.OnCrystalsCleared += ClearSpellInfo;
        CombatManager.OnCrystalsManuallyCleared += ClearSpellInfo;
    }

    private void OnDestroy()
    {
        PlayerStats.OnPlayerDamaged -= UpdateHealthBar;
        DamageCrystal.OnCrystalCollected -= UpdateUltimateBar;
        CombatManager.OnUltimateCast -= UpdateUltimateBar;
        CombatManager.OnCrystalsCleared -= ClearSpellInfo;
        CombatManager.OnCrystalsManuallyCleared -= ClearSpellInfo;

        Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        combatManager = CombatManager.Instance;

        decorCoroutines = new Coroutine[healthBarDecor.Count];
        decorStates = new bool[healthBarDecor.Count];

        UpdateHealthBar(0, CombatManager.Instance?.GetPlayerDamagable());

        // Reset ultimate bar
        UpdateUltimateBar(null);
    }

    public void VictoryCanvas(GameObject p, GameObject c, int exp, int damageD, int damageT, int ing, int time)
    {
        experienceEarned = exp;
        damageDealt = damageD;
        damageTaken = damageT;
        ingredientsCollected = ing;
        timeTaken = time;

        StartCoroutine(VictorySequence(p, c));
    }

    private IEnumerator VictorySequence(GameObject player, GameObject victoryCam)
    {
        // Allow brief period of movement after last enemy has died
        yield return new WaitForSeconds(1f);

        player.GetComponent<CombatMovement>().canMove = false;
        Debug.Log("Player should not be able to move!");

        // Period to let player register they have won
        yield return new WaitForSeconds(2f);

        // Disable other UI
        ToggleCombatUIVisibility(false);

        // Enable Victory UI
        victoryUI.SetActive(true);

        // Destroy old camera, create new one
        Destroy(GameObject.FindGameObjectWithTag("MainCamera"));
        GameObject vCam = Instantiate(victoryCam);
        vCam.transform.parent = player.transform;
        vCam.transform.localPosition = new Vector3(2f, .3f, 4f);
        vCam.transform.localEulerAngles = new Vector3(0f, 180f, 0f);


        // Update Victory UI
        victoryUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("XP Earned: " + experienceEarned);
        victoryUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Damage Dealt: " + damageDealt);
        victoryUI.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText("Damage Taken: " + damageTaken);
        victoryUI.transform.GetChild(5).GetComponent<TextMeshProUGUI>().SetText("Ingredients Collected: " + ingredientsCollected);

        // Calculate time
        int minutes = timeTaken / 60;
        int seconds = timeTaken % 60;

        string min = minutes.ToString();
        string sec = seconds.ToString();
        if(minutes < 10)
        {
            min = "0" + min;
        }
        if (seconds < 10)
        {
            sec = "0" + sec;
        }


        victoryUI.transform.GetChild(6).GetComponent<TextMeshProUGUI>().SetText("Time Taken: " + min + ":" + sec);

        yield return new WaitForSeconds(returnToOverworldTime);

        StaticOverworldData.loadingFromCombat = true;
        SceneManager.LoadScene("CyreneTestScene");

        // Unlock player mouse
        Cursor.lockState = CursorLockMode.None;
        yield return null;
    }

    public void DefeatCanvas(GameObject p)
    {
        StartCoroutine(DefeatSequence(p));
    }

    private IEnumerator DefeatSequence(GameObject player)
    {
        // Allow brief period of movement after last enemy has died
        yield return new WaitForSeconds(1f);



        player.GetComponent<CombatMovement>().canMove = false;
        Debug.Log("Player should not be able to move!");

        // Period to let player register they have won
        yield return new WaitForSeconds(2f);

        // Disable other UI
        healthBarHolder.SetActive(false);
        ingredientText.SetActive(false);

        // Enable Victory UI
        victoryUI.SetActive(true);

        // Destroy old camera, create new one
        Destroy(GameObject.FindGameObjectWithTag("MainCamera"));
        GameObject vCam = null;
        vCam.transform.parent = player.transform;
        vCam.transform.localPosition = new Vector3(2f, .3f, 4f);
        vCam.transform.localEulerAngles = new Vector3(0f, 180f, 0f);


        // Update Victory UI
        victoryUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("XP Earned: " + experienceEarned);
        victoryUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Damage Dealt: " + damageDealt);
        victoryUI.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText("Damage Taken: " + damageTaken);
        victoryUI.transform.GetChild(5).GetComponent<TextMeshProUGUI>().SetText("Ingredients Collected: " + ingredientsCollected);

        // Calculate time
        int minutes = timeTaken / 60;
        int seconds = timeTaken % 60;

        string min = minutes.ToString();
        string sec = seconds.ToString();
        if (minutes < 10)
        {
            min = "0" + min;
        }
        if (seconds < 10)
        {
            sec = "0" + sec;
        }


        victoryUI.transform.GetChild(6).GetComponent<TextMeshProUGUI>().SetText("Time Taken: " + min + ":" + sec);

        yield return new WaitForSeconds(returnToOverworldTime);

        StaticOverworldData.loadingFromCombat = true;
        SceneManager.LoadScene("CyreneTestScene");
        
        // Unlock player mouse
        Cursor.lockState = CursorLockMode.None;
        yield return null;
    }

    private void UpdateHealthBar(int damage, IDamagable player)
    {
        //healthbarImage.material.SetFloat(MASK_PERCENT, (float)player.Stats.CurrentHealth / (float)player.Stats.MaxHealth);

        float currentHealthPercentage = (float)player.Stats.CurrentHealth / player.Stats.MaxHealth;

        healthSlider.value = currentHealthPercentage;

        currentHealthText.text = $"{player.Stats.CurrentHealth}/";
        maxHealthText.text = player.Stats.MaxHealth.ToString();

        // TODO CYRENE: Figure out a better way to set damageSlider.value to healthSlider.value
        // when combat starts
        if (damage == 0)
        {
            Debug.Log(currentHealthPercentage);
            CalculateDecorState(currentHealthPercentage, true);
            damageSlider.value = player.Stats.CurrentHealth / (float)player.Stats.MaxHealth;
            return;
        }

        CalculateDecorState(currentHealthPercentage, false);

        if (damageBarAnimateCoroutine != null)
            StopCoroutine(damageBarAnimateCoroutine);

        damageBarAnimateCoroutine = StartCoroutine(
            AnimateDamageBar(player.Stats.CurrentHealth / (float)player.Stats.MaxHealth));
    }

    private IEnumerator AnimateDamageBar(float toPercent)
    {
        float currVal = damageSlider.value;
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / damageSliderAnimateTime;

            damageSlider.value = Mathf.Lerp(currVal, toPercent, damageSliderAnimCurve.Evaluate(progress));
        }

        damageBarAnimateCoroutine = null;
    }

    private void CalculateDecorState(float currentHealthPercent, bool immediate)
    {
        for (int i = decorPercentages.Count - 1; i >= 0; i--)
        {
            if(immediate)
            {
                decorCoroutines[i] = StartCoroutine(ScaleDecor(healthBarDecor[i],
                    currentHealthPercent >= decorPercentages[i], immediate));

                decorStates[i] = currentHealthPercent >= decorPercentages[i];
                continue;
            }

            if(currentHealthPercent >= decorPercentages[i] != decorStates[i])
            {
                decorStates[i] = currentHealthPercent >= decorPercentages[i];

                if (decorCoroutines[i] != null)
                    StopCoroutine(decorCoroutines[i]);

                decorCoroutines[i] = StartCoroutine(ScaleDecor(healthBarDecor[i],
                    currentHealthPercent >= decorPercentages[i], immediate));
            }
        }
    }

    private IEnumerator ScaleDecor(GameObject decor, bool scaleUp, bool immediate)
    {
        if(immediate)
        {
            decor.transform.localScale = Vector3.one * (scaleUp ? 1f : 0f);
            yield break;
        }    

        float fromScale = decor.transform.localScale.x;
        float toScale = scaleUp ? 1f : 0f;

        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / 1f;

            decor.transform.localScale = Mathf.Lerp(fromScale, toScale, progress) * Vector3.one;
            decor.transform.Rotate(0f, 0f, Time.deltaTime * 250f);
        }
    }

    private void UpdateUltimateBar(CombatCrystal ingredient) { UpdateUltimateBar(); }

    private void UpdateUltimateBar()
    {
        if (CombatManager.resonanceCharge < CombatManager.resonanceChargeMax)
            ultimateText.text = $"Resonance: {CombatManager.resonanceCharge} / {CombatManager.resonanceChargeMax}";
        else
            ultimateText.text = $"Resonance: PRIMED";

        if (ultimateBarAnimateCoroutine != null)
            StopCoroutine(ultimateBarAnimateCoroutine);

        ultimateBarAnimateCoroutine = StartCoroutine(
            AnimateUltimateBar((float)CombatManager.resonanceCharge / CombatManager.resonanceChargeMax));
    }

    private IEnumerator AnimateUltimateBar(float toPercent)
    {
        float currVal = ultimateSlider.value;
        float progress = 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / ultimateSliderAnimateTime;

            ultimateSlider.value = Mathf.Lerp(currVal, toPercent, ultimateSliderAnimCurve.Evaluate(progress));
        }

        ultimateBarAnimateCoroutine = null;
    }
    
    public void SetPopup(string text, float textDisplayTime)
    {
        popupText.text = text;

        if(popupAlphaAnimateCoroutine != null)
            StopCoroutine(popupAlphaAnimateCoroutine);

        popupAlphaAnimateCoroutine = StartCoroutine(AnimatePopupAlpha(true, textDisplayTime));
    }

    private IEnumerator AnimatePopupAlpha(bool becomeVisible, float textDisplayTime = 0f)
    {
        float progress = 0f;
        float startAlpha = becomeVisible ? 0f : 1f;
        float toAlpha = becomeVisible ? 1f : 0f;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / ultimateSliderAnimateTime;

            popupCanvasGroup.alpha = Mathf.Lerp(startAlpha, toAlpha, popupAlphaAnimCurve.Evaluate(progress));
        }

        yield return GameFlowUtility.WaitForGameplaySeconds(textDisplayTime);

        if (becomeVisible)
            popupAlphaAnimateCoroutine = StartCoroutine(AnimatePopupAlpha(false));
        else
            popupAlphaAnimateCoroutine = null;
    }

    public void ToggleCombatUIVisibility(bool val)
    {
        combatCanvasGroup.alpha = val ? 1f : 0f;
    }

    public void ClearSpellInfo()
    {
        crystalsCollectedText.GetComponent<TextMeshProUGUI>().text = "";
    }

    private void OnValidate()
    {
        if(decorPercentages.Count > healthBarDecor.Count)
        {
            decorPercentages.RemoveRange(healthBarDecor.Count, decorPercentages.Count);
        }
    }
}
