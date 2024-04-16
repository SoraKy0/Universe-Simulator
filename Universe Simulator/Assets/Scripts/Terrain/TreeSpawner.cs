using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab; // Prefab of the tree object
    public int numberOfTrees = 50; // Number of trees to spawn
    public float squareSize = 100f; // Size of the square area to spawn trees
    public float maxHeight = 100f; // Maximum height from which raycasts will originate

    // Method to spawn trees
    public void SpawnTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            // Randomly generate a point within the square area
            float x = Random.Range(-squareSize / 2, squareSize / 2);
            float z = Random.Range(-squareSize / 2, squareSize / 2);
            Vector3 raycastOrigin = new Vector3(x, maxHeight, z);

            // Spawn a raycast downwards from the generated point
            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, Vector3.down, out hit))
            {
                // Instantiate a tree prefab at the hit point
                GameObject tree = Instantiate(treePrefab, hit.point, Quaternion.identity);
                // Optionally, you can rotate the tree randomly to add variation
                tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            }
        }
    }
}
