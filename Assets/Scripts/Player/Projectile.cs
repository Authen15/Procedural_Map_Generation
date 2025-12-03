using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float force;
    public Rigidbody rb;
    // Cinemachine.CinemachineImpulseSource source;

    private Creature _attacker;
    private Action<Creature> _onHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.position;
    }

    public void Fire(Creature attacker, Action<Creature> onHitCallback)
    {
        _attacker = attacker;
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
        _onHit = onHitCallback;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Projectile"))
        {
            rb.isKinematic = true;
            StartCoroutine(Countdown());

            //TODO REMOVE, DEBUG PURPOSE ONLY
            gameObject.name += " (" + collision.gameObject.name + ")";


            Creature creature = collision.gameObject.GetComponent<Creature>();
            if (creature != null)
            {
                creature.OnHit(_attacker);
                _onHit?.Invoke(creature);
            }
        }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }


}
