using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int xSize = 120;
    [SerializeField] int zSize = 120;

    [SerializeField] float noiseScale;
    [SerializeField] float heightMultiplier;

    [SerializeField] int xOffset;
    [SerializeField] int zOffset;

    [SerializeField] int octavesAmount = 4;
    [SerializeField] float lacunarity;
    [SerializeField] float persistence;

    [SerializeField] Gradient TerrainGradient;
    [SerializeField] Material mat;


    private Mesh mesh;
    private Texture2D gradientTexture;
    private Vector3[] vertices;

    private float[,] falloffMap;

    void Start()
    {
        CreateMesh();
        GradientToTexture();
        GenerateFalloffMap();
        GenerateTerrain();

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

                // Apply falloff map
                float falloffValue = falloffMap[x, z];
                yPos -= falloffValue;

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

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}