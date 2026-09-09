using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float zoomSmoothTime = 0.1f;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 1.5f;

   private float originalTopRadius;
private float originalCenterRadius;
private float originalBottomRadius;

private float targetZoom = 1f;
private float zoomVelocity;

private void Start()
{
    if (orbitalFollow == null)
    {
        Debug.LogError("PlayerCameraController: Orbital Follow is not assigned!");
        return;
    }

    originalTopRadius = orbitalFollow.Orbits.Top.Radius;
    originalCenterRadius = orbitalFollow.Orbits.Center.Radius;
    originalBottomRadius = orbitalFollow.Orbits.Bottom.Radius;
}

private void Update()
{
    HandleZoom();
}

private void HandleZoom()
{
    float scroll = Input.mouseScrollDelta.y;

    if (Mathf.Abs(scroll) > 0.01f)
    {
        targetZoom -= scroll * zoomSpeed;

        targetZoom = Mathf.Clamp(
            targetZoom,
            minZoom,
            maxZoom
        );
    }

    float currentZoom =
        orbitalFollow.Orbits.Center.Radius / originalCenterRadius;

    float newZoom = Mathf.SmoothDamp(
        currentZoom,
        targetZoom,
        ref zoomVelocity,
        zoomSmoothTime
    );

    orbitalFollow.Orbits.Top.Radius =
        originalTopRadius * newZoom;

    orbitalFollow.Orbits.Center.Radius =
        originalCenterRadius * newZoom;

    orbitalFollow.Orbits.Bottom.Radius =
        originalBottomRadius * newZoom;
}
}