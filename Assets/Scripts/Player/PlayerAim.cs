using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    public GameObject StandardCamera;
    public GameObject AimCamera;
    public GameObject AimReticle;

    public GameObject ProjectilePrefab;

    public Transform Arm;
    public Transform FireTransform;

    public float FireRate = 2;
    private float _lastFireTime = 0;

    private Creature _creature;
    private CreatureStats _playerStats;

    private Quaternion _originalArmLocalRotation;
    private float _aimUpAxis;
    private bool _isAiming;
    private bool _isAttacking;

    public void OnLook(InputAction.CallbackContext context)
    {
        _aimUpAxis = context.ReadValue<Vector2>().y;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValue<float>() > 0;
        ToggleCrosshair();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _isAttacking = context.ReadValue<float>() > 0;
    }

    public void Awake()
    {
        _originalArmLocalRotation = Arm.localRotation;

        _creature = GetComponent<Creature>();
        _playerStats = _creature.Stats;
    }


    public void Update()
    {
        // If pressed, rotate arm up to player's forward position; if released, reset to original local rotation
        if (_isAiming)
        {
            Arm.up = transform.forward;
            // Arm.Rotate(Arm.right, _aimUpAxis); // Adjust rotation based on _aimUpAxis
            Arm.localRotation *= Quaternion.AngleAxis(_aimUpAxis * 1f, Vector3.right);

            if (!AimCamera.activeInHierarchy)
            {
                StandardCamera.SetActive(false);
                AimCamera.SetActive(true);

                //Allow time for the camera to blend before enabling the UI
                StartCoroutine(ToggleCrosshair());
            }

            if (_isAttacking)
            {
                if ((Time.time - _lastFireTime) > (1f / _playerStats.AttackSpeed.Value)) {
                    Fire();
                    _lastFireTime = Time.time;
                }
            }
        }
        else
        {
            Arm.localRotation = _originalArmLocalRotation;

            if (!StandardCamera.activeInHierarchy)
            {
                StandardCamera.SetActive(true);
                AimCamera.SetActive(false);
                AimReticle.SetActive(false);
            }
        }
    }



    IEnumerator ToggleCrosshair()
    {
        yield return new WaitForSeconds(0.25f);
        AimReticle.SetActive(enabled);
    }



    void Fire()
    {
        Camera camera = Camera.main;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 1000f; // une grande distance si rien n'est touché
        }

        Vector3 direction = (targetPoint - FireTransform.position).normalized;

        GameObject projectile = Instantiate(ProjectilePrefab, FireTransform.position, Quaternion.LookRotation(direction));

        Action<Creature> callback = GetComponent<CreatureEffectManager>().ApplyOnHitEffects;
        projectile.GetComponent<Projectile>().Fire(_creature, callback);
    }
}
