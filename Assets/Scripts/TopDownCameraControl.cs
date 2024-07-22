using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TopDownCameraControl : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float zoomSpeed = 20f;
    public Transform playerTransform;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;
    private float initialCameraDistance;

    void Start()
    {
        // Récupère le transposer pour contrôler la distance de la caméra
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        initialCameraDistance = transposer.m_CameraDistance;
    }

    void Update()
    {
        // Oriente le joueur et la caméra vers le curseur
        Vector3 mousePos = Input.mousePosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerTransform.position);
        Vector2 direction = (mousePos - screenPoint).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        playerTransform.rotation = Quaternion.Euler(0, angle, 0);

        // Si le bouton M2 est appuyé, avance la caméra dans la direction de la souris
        if (Input.GetMouseButton(1))
        {
            transposer.m_CameraDistance -= zoomSpeed * Time.deltaTime;
        }
        else
        {
            transposer.m_CameraDistance = Mathf.Lerp(transposer.m_CameraDistance, initialCameraDistance, Time.deltaTime * rotationSpeed);
        }
    }
}
