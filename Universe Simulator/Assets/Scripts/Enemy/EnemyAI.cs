using UnityEngine;
using FishNet;
using FishNet.Object;

public class EnemyAI : NetworkBehaviour
{
    // Range within which the enemy can detect the player
    [SerializeField] private float playerDetectionRange = 10f;

    // Reference to the player that the enemy is currently targeting
    private Transform targetPlayer;
    // Speed at which the enemy moves towards the player
    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        // If not on the client, do end the Update method
        if (!IsClient)
            return;

        // If no player is targeted or the targeted player is out of detection range, find the closest player
        if (targetPlayer == null || Vector3.Distance(transform.position, targetPlayer.position) > playerDetectionRange)
            FindClosestPlayer();
        else
            MoveTowardsPlayer(); // Otherwise, move towards the targeted player
    }

    // Finds the closest player within the detection range
    private void FindClosestPlayer()
    {
        // Find all game objects tagged as "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float shortestDistance = Mathf.Infinity;
        Transform nearestPlayer = null;

        // Loop through each player to find the one closest to the enemy
        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player.transform;
            }
        }

        // Set the closest player as the target
        targetPlayer = nearestPlayer;
    }

    // Moves the enemy towards the targeted player
    private void MoveTowardsPlayer()
    {
        // If a player is targeted
        if (targetPlayer != null)
        {
            // Calculate the direction towards the player
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            // Move the enemy towards the player at the specified speed
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
