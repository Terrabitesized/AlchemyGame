using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class OverworldCameraManager : MonoBehaviour
{
    [Header("Dash Feedback")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float dashFOVIncrease = 6f;
    [SerializeField] private float dashFOVInTime = 0.05f;
    [SerializeField] private float dashFOVOutTime = 0.15f;

    private float normalFOV;

    private void OnEnable()
    {
        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();

        if (cinemachineCamera != null)
            normalFOV = cinemachineCamera.Lens.FieldOfView;

        OverworldMovement.OnSprintStarted += DashCameraFOV;
        OverworldMovement.OnSprintEnded += ResetCameraFOV;
    }

    private void OnDisable()
    {
        OverworldMovement.OnSprintStarted -= DashCameraFOV;
        OverworldMovement.OnSprintEnded -= ResetCameraFOV;
    }

    public void DashCameraFOV()
    { StartCoroutine(AnimateCameraFOVCoroutine(dashFOVIncrease, dashFOVInTime)); }

    public void AnimateCameraFOV(float fovChange, float animationDuration)
    { StartCoroutine(AnimateCameraFOVCoroutine(fovChange, animationDuration)); }

    private IEnumerator AnimateCameraFOVCoroutine(float fovChange, float animationDuration)
    {
        float elapsed = 0f;

        while (elapsed < dashFOVInTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / animationDuration;

            cinemachineCamera.Lens.FieldOfView =
                Mathf.Lerp(normalFOV, normalFOV + fovChange, t);

            yield return null;
        }
    }

    public void ResetCameraFOV()
    { StartCoroutine(ResetCameraFOVCoroutine()); }

    private IEnumerator ResetCameraFOVCoroutine()
    {
        float elapsed = 0f;
        float currentFOV = cinemachineCamera.Lens.FieldOfView;

        while (elapsed < dashFOVInTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / dashFOVOutTime;

            cinemachineCamera.Lens.FieldOfView =
                Mathf.Lerp(currentFOV, normalFOV, t);

            yield return null;
        }
    }

    public void AnimateCameraDuringDialogue(bool animateIn) { StartCoroutine(AnimateCameraDuringDialogueCoroutine(animateIn)); }

    private IEnumerator AnimateCameraDuringDialogueCoroutine(bool animateIn)
    {
        yield return null;
    }
}
