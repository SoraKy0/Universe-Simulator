using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int xSize = 10;
    [SerializeField] int zSize = 10;

    [SerializeField] float noiseScale = 0.05f;
    [SerializeField] float heightMultiplier = 10;
    [SerializeField] int xOffset;
    [SerializeField] int zOffset;

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

    private void GenerateTerrain() 
    {
        //Creates vertices for the mesh
        vertices = new Vector3[(xSize + 1) * (zSize + 1)]; 

        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float yPos = Mathf.PerlinNoise((x + xOffset) * noiseScale, (z + zOffset) * noiseScale) * heightMultiplier;
                vertices[i] = new Vector3(x, yPos, z);
                i++;
            }
        }

        //Creates Triangles
        int[] triangles = new int[xSize * zSize * 6];

        int vertex = 0;
        int triangleIndex = 0;

        for(int z = 0; z < zSize; z++)
        {
            for(int x = 0; x < xSize; x++)     
            {
                triangles[triangleIndex + 0] = vertex + 0;
                triangles[triangleIndex + 1] = vertex + xSize + 1;
                triangles[triangleIndex + 2] = vertex + 1;

                triangles[triangleIndex + 3] = vertex + 1;
                triangles[triangleIndex + 4] = vertex + xSize + 1;
                triangles[triangleIndex + 5] = vertex + xSize + 2;

                vertex++;
                triangleIndex += 6;
            }       
            vertex++;
        }
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}