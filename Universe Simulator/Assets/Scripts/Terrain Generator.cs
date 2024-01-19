using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int xSize = 10;
    [SerializeField] int zSize = 10;

    private Mesh mesh;

    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }



    void Update()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)]; //This specifies the size of vertices array

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
}

//https://www.youtube.com/watch?v=hNRFosb_3Tc