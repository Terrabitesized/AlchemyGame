using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatCanvas : MonoBehaviour
{
    public static CombatCanvas Instance;

    [Header("Health Bar UI")]
    [SerializeField] private GameObject healthBarHolder;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthbarImage;
    private readonly int MASK_PERCENT = Shader.PropertyToID("_MaskPercent");
    [SerializeField] private AnimationCurve damageSliderAnimCurve;
    [SerializeField] private Slider damageSlider;
    [SerializeField] private float damageSliderAnimateTime = .5f;
    private Coroutine damageBarAnimateCoroutine;

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
    [SerializeField] private GameObject spellNameText;
    [SerializeField] private GameObject spellDescriptionText;
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
        PotionManager.OnSpellPrimed += DisplaySpellInfo;
        PotionManager.OnSpellCast += ClearSpellInfo;
        PotionManager.OnSpellFail += ClearSpellInfo;

        PlayerStats.OnPlayerDamaged += UpdateHealthBar;
        IngredientScript.OnIngredientCollected += UpdateUltimateBar;
        CombatManager.OnUltimateCast += UpdateUltimateBar;
        CombatManager.OnIngredientsManuallyCleared += ClearSpellInfo;
    }

    private void OnDestroy()
    {
        PotionManager.OnSpellPrimed -= DisplaySpellInfo;
        PotionManager.OnSpellCast -= ClearSpellInfo;
        PotionManager.OnSpellFail -= ClearSpellInfo;

        PlayerStats.OnPlayerDamaged -= UpdateHealthBar;
        IngredientScript.OnIngredientCollected -= UpdateUltimateBar;
        CombatManager.OnUltimateCast -= UpdateUltimateBar;
        CombatManager.OnIngredientsManuallyCleared -= ClearSpellInfo;

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
        healthBarHolder.SetActive(false);
        ingredientText.SetActive(false);

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
        healthbarImage.material.SetFloat(MASK_PERCENT, (float)player.Stats.CurrentHealth / (float)player.Stats.MaxHealth);
        //healthSlider.value = (float) player.Stats.CurrentHealth / (float) player.Stats.MaxHealth;
        healthText.text = $"HP: {player.Stats.CurrentHealth} / {player.Stats.MaxHealth}";

        // TODO CYRENE: Figure out a better way to set damageSlider.value to healthSlider.value
        // when combat starts
        if (damage == 0)
        {
            damageSlider.value = player.Stats.CurrentHealth / (float)player.Stats.MaxHealth;
            return;
        }

        // TODO CYRENE: Create coroutine to animate damageSlider.Value from currentValue to healthSlider.Value
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

    private void UpdateUltimateBar(CombatIngredient ingredient) { UpdateUltimateBar(); }

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

    public void DisplaySpellInfo(Spell spell)
    {
        if(spell != null)
        {
            spellNameText.GetComponent<TextMeshProUGUI>().text = spell.spellName;
            spellDescriptionText.GetComponent<TextMeshProUGUI>().text = spell.spellDescription;
        }
        else
        {
            spellNameText.GetComponent<TextMeshProUGUI>().text = "Invalid Spell Combo!";
            spellDescriptionText.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void ClearSpellInfo(Spell spell)
    {
        spellNameText.GetComponent<TextMeshProUGUI>().text = "";
        spellDescriptionText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void ClearSpellInfo()
    {
        spellNameText.GetComponent<TextMeshProUGUI>().text = "";
        spellDescriptionText.GetComponent<TextMeshProUGUI>().text = "";
    }
}
