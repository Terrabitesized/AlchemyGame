using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{

    private Camera cam;


    [Header("Animation Variables")]
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightCurve;
    [SerializeField] private float textHeight;

    private TextMeshProUGUI tmp;
    private float time = 0;
    private Vector3 origin;

    private void Awake()
    {
        cam = Camera.main;
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // For billboarding towards the camera
        transform.forward = cam.transform.forward;

        // Text animation
        tmp.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));
        transform.localScale = Vector3.one * scaleCurve.Evaluate(time);
        transform.position = new Vector3(origin.x, textHeight + heightCurve.Evaluate(time), origin.z);
        time += Time.deltaTime;

    }
}
