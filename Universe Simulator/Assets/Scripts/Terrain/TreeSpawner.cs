using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;

public class TreeSpawner : NetworkBehaviour
{
    public GameObject treePrefab; //

    
    public int numOfTree = 350; //Number of trees that will spawn divide by 4 (because the treeSpawnArea is not positions properly)
    public float treeSpawnArea = 245f; //The area where the trees will spawn
    public float rayCastSpawnHeight = 18f; //The height where the raycasts will spawn

    // The points where the trees will spawn will be synced over the network where each players will have the trees to spawn in the same areas
    [SyncObject]
    private readonly SyncList<Vector3> treeSpawnLocations = new SyncList<Vector3>();

    //Shoots raycasts randomly in side the treeSpawnArea and places the tree prefab there
    public void SpawnTrees()
    {
        if (IsServer)
        {
            treeSpawnLocations.Clear(); // Clear the list of spawn locations so each time the game is ran the old tree spawn data will clear

            for (int i = 0; i < numOfTree; i++)
            {
                // Randomly generate a point within the square area
                float x = Random.Range(-treeSpawnArea / 2, treeSpawnArea / 2);
                float z = Random.Range(-treeSpawnArea / 2, treeSpawnArea / 2);
                Vector3 raycastSpawnPoint = new Vector3(x, rayCastSpawnHeight, z);

                // Spawn a raycast downwards from the created point
                RaycastHit hit;
                if (Physics.Raycast(raycastSpawnPoint, Vector3.down, out hit))
                {
                    // Check if the hit point's y-coordinate is at or above 0 so that trees do not spawn in the water
                    if (hit.point.y >= 0)
                    {
                        // Store the spawn location in the list
                        treeSpawnLocations.Add(hit.point);

                        // Instantiate a tree prefab at the hit point
                        GameObject tree = Instantiate(treePrefab, hit.point, Quaternion.identity);
                        // Optionally, rotate the tree randomly to make the trees look natural as if they all faced the same way it will look strange
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    }
                }
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsClient) 
        {
            // Spawn trees based on synchronized treeSpawnLocations on clients
            foreach (var location in treeSpawnLocations)
            {
                GameObject tree = Instantiate(treePrefab, location, Quaternion.identity);
                tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            }
        }
    }

    // Uses Gizmo to draw lines to show the area that the trees can spawn in
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // uses the position of the gameobject as the center and then movess the treeSpawnarea around for each side
        Vector3 center = transform.position;
        Vector3 topLeft = center + new Vector3(-treeSpawnArea / 2, 0, treeSpawnArea / 2);
        Vector3 topRight = center + new Vector3(treeSpawnArea / 2, 0, treeSpawnArea / 2);
        Vector3 bottomRight = center + new Vector3(treeSpawnArea / 2, 0, -treeSpawnArea / 2);
        Vector3 bottomLeft = center + new Vector3(-treeSpawnArea / 2, 0, -treeSpawnArea / 2);

        // draw a square using the points created and linking them together 
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
