using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    public GameObject ProjectilePrefab;

    public Transform Arm;
    public Transform FireTransform;

    public float FireRate = 2;
    private float _lastFireTime = 0;

    private Creature _creature;
    private CreatureStats _playerStats;

    private float _lookUpAxis;
    private bool _isAttacking;

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookUpAxis = context.ReadValue<Vector2>().y;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _isAttacking = context.ReadValue<float>() > 0;
    }

    public void Awake()
    {
        _creature = GetComponent<Creature>();
        _playerStats = _creature.Stats;
    }

    public void Update()
    {
        Arm.up = transform.forward;
        Arm.localRotation *= Quaternion.AngleAxis(_lookUpAxis, Vector3.right);

        if (_isAttacking)
        {
            if ((Time.time - _lastFireTime) > (1f / _playerStats.AttackSpeed.Value))
            {
                Fire();
                _lastFireTime = Time.time;
            }
        }
    }

    public Vector3 GetAimingPoint()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // aim at FireTransform point height
        Plane groundPlane = new Plane(Vector3.up, FireTransform.position);

        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return Vector3.zero; // we shouldn't get there as the ray whould always hit the plane
    }

    void Fire()
    {
        Vector3 firePoint = GetAimingPoint();
        Vector3 fireDir = (firePoint - FireTransform.position).normalized;

        GameObject projectile = Instantiate(ProjectilePrefab, FireTransform.position, Quaternion.LookRotation(fireDir));

        Action<Creature> callback = GetComponent<CreatureEffectManager>().ApplyOnHitEffects;
        projectile.GetComponent<Projectile>().Fire(_creature, callback);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(FireTransform.position, GetAimingPoint());
        Gizmos.DrawSphere(GetAimingPoint(), 0.1f);


        // // #### Draw the plane at aiming height ####
        // float size = 10f;

        // Vector3 center = FireTransform.position;

        // // Draw the plane
        // Vector3 p0 = center + new Vector3(-size, 0, -size);
        // Vector3 p1 = center + new Vector3(size, 0, -size);
        // Vector3 p2 = center + new Vector3(size, 0, size);
        // Vector3 p3 = center + new Vector3(-size, 0, size);

        // Gizmos.DrawLine(p0, p1);
        // Gizmos.DrawLine(p1, p2);
        // Gizmos.DrawLine(p2, p3);
        // Gizmos.DrawLine(p3, p0);

        // // draw a cross in the middle
        // Gizmos.DrawLine(center + Vector3.forward * size, center - Vector3.forward * size);
        // Gizmos.DrawLine(center + Vector3.right * size, center - Vector3.right * size);
    }
}
