using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class EnemyAI : NetworkBehaviour
{
    [SyncVar]
    private Transform targetPlayer;

    public float speed = 5f;

    void Update()
    {
        if (!IsClient)
            return;

        if (targetPlayer == null)
            FindPlayer();
        else
            MoveTowardsPlayer();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            targetPlayer = playerObject.transform;
        }
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
