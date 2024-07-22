using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    [Tooltip("Only objects on this layer will be hit by the ray.")]
    public LayerMask raycastLayer;

    [Header("Camera Settings")]
    public Camera mainCamera;
    public float rotationSpeed;
    public float maxAimDistance = 8f;
    public float aimSensitivity = 5f;
    public float aimSensitivityMultiplier = 3f;

    private Vector3 cursorPosition;
    private bool cursorPositionUpdated = false;

    private void Start()
    {
        InitCameraSettings();
    }

    private void InitCameraSettings()
    {
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        UpdateCursorWorldPosition();
        RotatePlayerTowardsCursor();
        HandleCameraPosition();
    }

    private void UpdateCursorWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, raycastLayer))
        {
            cursorPosition = hit.point;
            cursorPositionUpdated = true;
        }
        else
        {
            cursorPositionUpdated = false;
        }
    }

    private void RotatePlayerTowardsCursor()
    {
        if (cursorPositionUpdated)
        {
            Vector3 directionToCursor = new Vector3(cursorPosition.x, player.position.y, cursorPosition.z) - player.position;
            orientation.forward = directionToCursor.normalized;
            playerObj.forward = orientation.forward;
        }
    }

    private void HandleCameraPosition()
    {
        if (Input.GetButton("Fire2"))
        {
            MoveCameraTowardsCursor(aimSensitivityMultiplier);
        }
        else if (cursorPositionUpdated)
        {
            MoveCameraTowardsCursor();
            ResetCameraPosition();
        }
    }

    private void MoveCameraTowardsCursor(float aimSensitivityMultiplier = 1f)
    {
        Vector3 targetPosition = cursorPosition;
        targetPosition.y = transform.position.y;
        targetPosition = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * aimSensitivity * aimSensitivityMultiplier);
        if(Vector3.Distance(transform.position, targetPosition) > maxAimDistance)
        {
            targetPosition = transform.position + orientation.forward * maxAimDistance;
        }
        mainCamera.transform.position = targetPosition;
    }

    private void ResetCameraPosition()
    {
        Vector3 targetPosition = mainCamera.transform.position;
        targetPosition = Vector3.Lerp(targetPosition, transform.position, Time.deltaTime);
        mainCamera.transform.position = targetPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cursorPosition, .1f);
        Gizmos.DrawLine(player.position, cursorPosition);
    }
}
