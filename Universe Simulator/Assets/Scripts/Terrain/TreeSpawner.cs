using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;

public class TreeSpawner : NetworkBehaviour
{
    public GameObject treePrefab; // Prefab of the tree object

    // Variables to control the spawning of trees (not synchronized)
    public int numOfTree = 350; // Number of trees to spawn
    public float treeSpawnArea = 245f; // Size of the square area to spawn trees
    public float rayCastSpawnHeight = 18f; // Maximum height from which raycasts will originate

    // A list of positions where trees have spawned, synchronized across the network
    [SyncObject]
    private readonly SyncList<Vector3> treeSpawnLocations = new SyncList<Vector3>();

    // Method to spawn trees
    public void SpawnTrees()
    {
        if (IsServer)
        {
            treeSpawnLocations.Clear(); // Clear the list of spawn locations

            for (int i = 0; i < numOfTree; i++)
            {
                // Randomly generate a point within the square area
                float x = Random.Range(-treeSpawnArea / 2, treeSpawnArea / 2);
                float z = Random.Range(-treeSpawnArea / 2, treeSpawnArea / 2);
                Vector3 raycastSpawnPoint = new Vector3(x, rayCastSpawnHeight, z);

                // Spawn a raycast downwards from the generated point
                RaycastHit hit;
                if (Physics.Raycast(raycastSpawnPoint, Vector3.down, out hit))
                {
                    // Check if the hit point's y-coordinate is at or above 0
                    if (hit.point.y >= 0)
                    {
                        // Store the spawn location in the list
                        treeSpawnLocations.Add(hit.point);

                        // Instantiate a tree prefab at the hit point
                        GameObject tree = Instantiate(treePrefab, hit.point, Quaternion.identity);
                        // Optionally, rotate the tree randomly to add variation
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    }
                }
            }
        }
    }

    // Method called when the client connects
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsServer)
        {
            // Randomize number of trees and spawn them
            numOfTree = 350;

            // Set other variables
            treeSpawnArea = 245f; // Fixed square size
            rayCastSpawnHeight = 18f; // Fixed maximum height

        }
        else
        {
            // Spawn trees based on synchronized treeSpawnLocations on clients
            foreach (var location in treeSpawnLocations)
            {
                GameObject tree = Instantiate(treePrefab, location, Quaternion.identity);
                tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            }
        }
    }

    // Gizmo visualization of the square area where trees will spawn
    private void OnDrawGizmos()
    {
        // Define the color for the gizmo
        Gizmos.color = Color.red;

        // Calculate the corners of the square area
        Vector3 center = transform.position;
        Vector3 topLeft = center + new Vector3(-treeSpawnArea / 2, 0, treeSpawnArea / 2);
        Vector3 topRight = center + new Vector3(treeSpawnArea / 2, 0, treeSpawnArea / 2);
        Vector3 bottomRight = center + new Vector3(treeSpawnArea / 2, 0, -treeSpawnArea / 2);
        Vector3 bottomLeft = center + new Vector3(-treeSpawnArea / 2, 0, -treeSpawnArea / 2);

        // Draw the square using lines
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
