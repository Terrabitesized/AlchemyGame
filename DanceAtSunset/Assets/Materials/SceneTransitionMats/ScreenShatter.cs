using System.Collections;
using UnityEngine;

public class ScreenShatter : MonoBehaviour
{
    public static ScreenShatter Instance;

    [SerializeField] GameObject explosionCenterObject;
    [SerializeField] Material explosionMaterial;
    private Vector3 explosionCenter = Vector3.zero;

    private bool screenHasShattered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        CombatManager.OnCombatStart += CallScreenShatter;
    }

    private void OnDisable()
    {
        CombatManager.OnCombatStart -= CallScreenShatter;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(TakeScreenshot());
        }
    }

    private void FixedUpdate()
    {
        if (!screenHasShattered)
            return;

        foreach (Transform t in transform)
        {
            //Debug.Log("Explode!");
            if (t.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddForce(Vector3.left * 30f);
            }
        }
    }

    public IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        Texture2D screenshotTexture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        screenshotTexture2D.ReadPixels(rect, 0, 0);
        screenshotTexture2D.Apply();

        explosionMaterial.SetTexture("_BaseMap", screenshotTexture2D);

        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }

    private void CallScreenShatter(int c)
    {
        StartCoroutine(PlayScreenShatterAnimation());
    }

    private IEnumerator PlayScreenShatterAnimation()
    {
        if (explosionCenterObject != null)
            explosionCenter = explosionCenterObject.transform.position;

        foreach (Transform t in transform)
        {
            if (t.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(100f, explosionCenter, 10f);
                childRigidbody.useGravity = true;
            }
        }

        yield return new WaitForSeconds(.33f);
        screenHasShattered = true;

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
