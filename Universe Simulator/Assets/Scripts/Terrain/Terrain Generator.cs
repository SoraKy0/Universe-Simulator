using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object.Synchronizing;

public class TerrainGenerator : NetworkBehaviour
{
    [SerializeField] int xSize = 120;
    [SerializeField] int zSize = 120;

    [SerializeField] int octavesAmount = 4;

    [SerializeField] Gradient TerrainGradient;
    [SerializeField] Material mat;


    private Mesh mesh;
    private Texture2D gradientTexture;
    private Vector3[] vertices;

    private float[,] falloffMap;

    [SyncVar] public float noiseScale;
    [SyncVar] public int xOffset;
    [SyncVar] public int zOffset;
    [SyncVar] public float lacunarity;
    [SyncVar] public float persistence;
    [SyncVar] public float heightMultiplier;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsServer)
        {
            noiseScale = Random.Range(0.02f, 0.03f);
            xOffset = Random.Range(0, 1000);
            zOffset = Random.Range(0, 1000);
            lacunarity = Random.Range(1.4f, 2f);
            persistence = Random.Range(0.5f, 0.4f);

            float heightMin = -8f;
            float heightMax = 10f;
            do
            {
                heightMultiplier = Random.Range(heightMin, heightMax);
            } while (heightMultiplier > -3.9f && heightMultiplier < 3);
        }
    }

    void Start()
    {
        CreateMesh();
        GradientToTexture();
        GenerateFalloffMap();
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

    void GenerateFalloffMap()
    {
        falloffMap = FalloffForTerrain.GenerateFalloffMap(xSize + 1);
    }

    
    //Generates the terrain geometry using Perlin noise and falloff map.
    private void GenerateTerrain()
    {
        // Initialize array to hold vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;

        // Loop through each grid point to create vertices
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float yPos = 0;

                // Use Perlin noise to calculate terrain height
                for (int y = 0; y < octavesAmount; y++)
                {
                    float frequency = Mathf.Pow(lacunarity, y);
                    float amplitude = Mathf.Pow(persistence, y);
                    yPos += Mathf.PerlinNoise((x + xOffset) * noiseScale * frequency,
                                              (z + zOffset) * noiseScale * frequency) * amplitude;
                }

                // Adjust terrain height based on falloff map
                float falloffValue = falloffMap[x, z];
                yPos -= falloffValue;

                // Apply height multiplier
                yPos *= heightMultiplier;

                // Assign vertex position
                vertices[i] = new Vector3(x, yPos, z);
                i++;
            }
        }

        // Initialize list to store triangle indices
        List<int> triangles = new List<int>();
        int vert = 0;

        // Loop through each grid cell to generate triangles
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // Define vertices for each triangle
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

        // Clear existing mesh data
        mesh.Clear();

        // Assign vertices and triangles to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();

        // Recalculate normals for proper lighting
        mesh.RecalculateNormals();

        // Update MeshCollider with new mesh
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

}