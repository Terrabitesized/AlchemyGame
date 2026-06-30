using System.Collections;
using UnityEngine;

public class ScreenShatter : MonoBehaviour
{
    public static ScreenShatter Instance;

    [SerializeField] GameObject explosionCenterObject;
    [SerializeField] Material explosionMaterial;
    private Vector3 explosionCenter = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PlayScreenShatter());
        }
    }

    public IEnumerator PlayScreenShatter()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        Texture2D screenshotTexture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        screenshotTexture2D.ReadPixels(rect, 0, 0);
        screenshotTexture2D.Apply();

        explosionMaterial.SetTexture("_BaseMap", screenshotTexture2D);

        if (explosionCenterObject != null)
            explosionCenter = explosionCenterObject.transform.position;

        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);

            Debug.Log("Explode!");
            if (t.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
                childRigidbody.AddExplosionForce(100f, explosionCenter, 10f);
        }

        yield return new WaitForSeconds(3f);
        Debug.Log("I SHOULD BE EXPLODING???");
        Destroy(gameObject);
    }
}
