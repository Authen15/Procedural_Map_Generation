using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float force;
    public Rigidbody rb;
    // Cinemachine.CinemachineImpulseSource source;

    private CreatureStats _attackerStats;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.position;
    }

    public void Fire(CreatureStats attackerStats)
    {
        _attackerStats = attackerStats;
        // rb.AddForce(transform.forward * (100 * Random.Range(1.3f, 1.7f)), ForceMode.Impulse);
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
        // source = GetComponent<Cinemachine.CinemachineImpulseSource>();
        // source.GenerateImpulse(Camera.main.transform.forward);
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
                creature.OnHit(_attackerStats);
            }
        }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }


}
