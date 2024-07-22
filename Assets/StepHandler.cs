using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StepHandler : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayer; // Set this to "Ground" layer in the inspector

    public Transform playerTransform; // Assuming the player is the parent of this object

    private void Start()
    {
        playerTransform = transform.parent; // Get the parent transform
        if (playerTransform == null)
        {
            Debug.LogError("StepHandler must be a child of a player object!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((groundLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer) // Check if the collided object is in the "Ground" layer
        {
            // Move player to the height of the collided object
            Vector3 newPosition = playerTransform.position;
            newPosition.y = other.bounds.max.y;
            playerTransform.position = newPosition;
        }
    }
}
