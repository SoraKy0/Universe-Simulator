using UnityEngine;
using FishNet;
using FishNet.Object;

public class EnemyAI : NetworkBehaviour
{
    [SerializeField] private float playerDetectionRange = 10f; 

    private Transform targetPlayer;
    public float speed = 5f;

    void Update()
    {
        if (!IsClient)
            return;

        if (targetPlayer == null || Vector3.Distance(transform.position, targetPlayer.position) > playerDetectionRange)
            FindClosestPlayer();
        else
            MoveTowardsPlayer();
    }

    private void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float shortestDistance = Mathf.Infinity;
        Transform nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player.transform;
            }
        }

        targetPlayer = nearestPlayer;
    }

    private void MoveTowardsPlayer()
    {
        if (targetPlayer != null)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}