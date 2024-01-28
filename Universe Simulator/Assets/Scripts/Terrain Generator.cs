using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int xSize = 10;
    [SerializeField] int zSize = 10;

    private Mesh mesh;
    private Vector3[] vertices;

    void Start()
    {
        CreateMesh();
    }

    void Update()
    {
        GenerateTerrain();
    }

    void CreateMesh() //Makes Mesh
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void GenerateTerrain() //Creates Vectors
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)]; 

        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        mesh.vertices = vertices;
    }

    private void OnDrawGizmos() //Draws Spheres at each Vectors to show 
    {
        foreach (Vector3 pos in vertices)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }
    }
}