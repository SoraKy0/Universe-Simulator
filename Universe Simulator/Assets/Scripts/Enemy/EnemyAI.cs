using FishNet.Object;
using UnityEngine;

public class EnemyAI : NetworkBehaviour
{
    public float speed = 5f; // The speed at which the enemy moves
    private Transform player; // Reference to the player's transform

    void Update()
    {
        // Only run this code on the server
        if (!IsServer)
            return;

        // Find the player by tag
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // If the player is found, move towards the player
        if (player != null)
        {
            // Calculate direction towards player
            Vector3 direction = (player.position - transform.position).normalized;

            // Move enemy towards the player
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
