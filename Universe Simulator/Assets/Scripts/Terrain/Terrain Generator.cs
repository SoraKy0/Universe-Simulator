using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int xSize = 10;
    [SerializeField] int zSize = 10;

    [SerializeField] float noiseScale = 0.05f;
    [SerializeField] float heightMultiplier = 2;
    [SerializeField] int xOffset;
    [SerializeField] int zOffset;
    [SerializeField] int octavesAmount = 1;
    [SerializeField] float lacunarity = 1f;
    [SerializeField] float persistence = 1f;

    [SerializeField] Gradient TerrainGradient;
    [SerializeField] Material mat;
    [SerializeField] PhysicMaterial physicMaterial; // Physics material

    private Mesh mesh;
    private Texture2D gradientTexture;

    private Vector3[] vertices;

    void Start()
    {
        CreateMesh();
        GradientToTexture();
        GenerateTerrain();
    }

    void Update()
    {
        GenerateTerrain();
        GradientToTexture();
        UpdateMaterialProperties();
    }

    private void UpdateMaterialProperties()
    {
        float minTerrainHeight = mesh.bounds.min.y + transform.position.y - 0.1f;
        float maxTerrainHeight = mesh.bounds.max.y + transform.position.y + 0.1f;

        mat.SetTexture("terrainGradient", gradientTexture);
        mat.SetFloat("minTerrainHeight", minTerrainHeight);
        mat.SetFloat("maxTerrainHeight", maxTerrainHeight);
    }

    private void GradientToTexture()
    {
        gradientTexture = new Texture2D(1, 100);
        Color[] pixelColors = new Color[100];

        for (int i = 0; i < 100; i++)
        {
            pixelColors[i] = TerrainGradient.Evaluate((float)i / 100);
        }

        gradientTexture.SetPixels(pixelColors);
        gradientTexture.Apply();
    }

    void CreateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void GenerateTerrain()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float yPos = 0;
                for (int y = 0; y < octavesAmount; y++)
                {
                    float frequency = Mathf.Pow(lacunarity, y);
                    float amplitude = Mathf.Pow(persistence, y);
                    yPos += Mathf.PerlinNoise((x + xOffset) * noiseScale * frequency, (z + zOffset) * noiseScale * frequency) * amplitude;
                }
                yPos *= heightMultiplier;
                vertices[i] = new Vector3(x, yPos, z);
                i++;
            }
        }

        List<int> triangles = new List<int>();
        int vert = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles.Add(vert + 0);
                triangles.Add(vert + xSize + 1);
                triangles.Add(vert + 1);
                triangles.Add(vert + 1);
                triangles.Add(vert + xSize + 1);
                triangles.Add(vert + xSize + 2);
                vert++;
            }
            vert++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Assign physics material to the MeshCollider
        GetComponent<MeshCollider>().material = physicMaterial;
    }
}
