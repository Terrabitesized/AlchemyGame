using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CrystalAura : MonoBehaviour
{
    public static CrystalAura Instance;

    [SerializeField] private GameObject auraCrystalGameObject;
    [SerializeField] private float crystalFireDelay = .5f;
    private List<CrystalProjectile> crystals = new List<CrystalProjectile>();
    private List<CrystalProjectile> firingCrystals = new List<CrystalProjectile>();
    private float radius = 1f;
    private float rotationSpeed = 100f;

    private int currentCrystalCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        CombatObjectPool.Instance?.CreatePool(auraCrystalGameObject);
    }

    private void OnEnable()
    {
        DamageCrystal.OnCrystalCollected += AddCrystal;
        CombatManager.OnSpellCast += FireCrystals;
        CombatManager.OnCrystalsManuallyCleared += ClearCrystals;
    }

    private void OnDisable()
    {
        DamageCrystal.OnCrystalCollected -= AddCrystal;
        CombatManager.OnSpellCast -= FireCrystals;
        CombatManager.OnCrystalsManuallyCleared -= ClearCrystals;

        Instance = null;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void AddCrystal(CombatCrystal crystal)
    {
        if (!CombatManager.Instance.CanCollectCrystals)
            return;

        CrystalProjectile crystalProjectile = CombatObjectPool.Instance.
            GetPooledObject(auraCrystalGameObject).GetComponent<CrystalProjectile>();

        if(crystalProjectile == null)
        {
            Debug.Log("OH MY GOD HTE PROJECTILE IS NULLLLLLLLLL");
        }

        crystalProjectile.gameObject.SetActive(true);
        crystals.Add(crystalProjectile);
        crystalProjectile.Initialize(crystal, crystals.Count - 2);
        crystalProjectile.transform.SetParent(transform);

        for(int i = 0; i < crystals.Count; i++)
        {
            float angle = i * Mathf.PI * 2f / crystals.Count;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            crystals[i].gameObject.transform.localPosition = new Vector3(x, 0f, z);
        }
    }

    private void FireCrystals()
    {
        firingCrystals = new List<CrystalProjectile>(crystals);
        crystals.Clear();

        StartCoroutine(FireCrystalDelay());
    }

    private IEnumerator FireCrystalDelay()
    {
        yield return new WaitForSeconds(crystalFireDelay);

        foreach (CrystalProjectile projectile in firingCrystals)
            projectile.Fire(CombatManager.Instance.GetNearestEnemy());

        ClearCrystals();
    }

    private void ClearCrystals()
    {
        firingCrystals.Clear();
        currentCrystalCount = 0;
    }

    public void ToggleCrystalVisibility(bool val)
    {
        foreach (CrystalProjectile projectile in crystals)
            projectile.gameObject.GetComponent<VisualEffect>().enabled = val;
    }
}
