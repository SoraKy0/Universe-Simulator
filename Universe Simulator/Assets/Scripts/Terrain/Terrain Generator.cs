using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object.Synchronizing;

public class TerrainGenerator : NetworkBehaviour
{
    [SerializeField] int xSize = 120;  // Width of the terrain 
    [SerializeField] int zSize = 120;  // Length of the terrain 

    [SerializeField] int octavesAmount = 4;  // Number of layers of Perlin noise

    [SerializeField] Gradient TerrainGradient;  
    [SerializeField] Material mat;  

    private Mesh mesh;  
    private Texture2D gradientTexture;  // Texture representing terrain gradient
    private Vector3[] vertices;  // Array to store vertices of the terrain

    private float[,] falloffMap;  // Array representing falloff map for terrain edges

    // Variables synchronized across network for consistent terrain generation
    [SyncVar] public float noiseScale;
    [SyncVar] public int xOffset;
    [SyncVar] public int zOffset;
    [SyncVar] public float lacunarity;
    [SyncVar] public float persistence;
    [SyncVar] public float heightMultiplier;

    public override void OnStartClient()
    {
        base.OnStartClient();
        // Randomize variables on the server to ensure all clients get the same variables
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

    // Executed once when the game starts
    void Start()
    {
        CreateMesh();
        GradientToTexture();
        GenerateFalloffMap();
        GenerateTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        GenerateTerrain();
        GradientToTexture();
        UpdateMaterialProperties();
    }

    // Update material properties of gradient texture and terrain height range
    private void UpdateMaterialProperties()
    {
        float minTerrainHeight = mesh.bounds.min.y + transform.position.y - 0.1f;
        float maxTerrainHeight = mesh.bounds.max.y + transform.position.y + 0.1f;

        mat.SetTexture("terrainGradient", gradientTexture);
        mat.SetFloat("minTerrainHeight", minTerrainHeight);
        mat.SetFloat("maxTerrainHeight", maxTerrainHeight);
    }

    // Generate gradient texture based on terrain gradient
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

    // Create the initial empty mesh for the terrain
    void CreateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Generate falloff map to lower the terrains edges to make the island
    void GenerateFalloffMap()
    {
        falloffMap = FalloffForTerrain.GenerateFalloffMap(xSize + 1);
    }

    // Generate terrain geometry using Perlin noise and falloff map
    private void GenerateTerrain()
    {
        // Initialize array to hold vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;

        // Loop through each point to create vertices
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
