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

    [SerializeField] int octavesAmount = 4;  // Number of layers of Perlin noise (just makes the ground more detailed)

    [SerializeField] Gradient TerrainGradient;
    [SerializeField] Material mat;

    private Mesh mesh;
    private Texture2D gradientTexture;  // Texture representing terrain gradient 
    private Vector3[] vertices;  // Array to store vertices of the terrain

    private float[,] falloffMap;  // Array representing falloff map for terrain edges

    //[SyncVar] syncs the variable for each player from server to clients 
    [SyncVar] public float noiseScale; //How Strong the Noise will be
    [SyncVar] public int xOffset; //Moves the perlin noise on the X-Axis
    [SyncVar] public int zOffset; //Moves the perlin noise on the Y-Axis
    [SyncVar] public float lacunarity; // Controls the frequency in the Octaves 
    [SyncVar] public float persistence; // Controls Decreace in the amplitude of octaves
    [SyncVar] public float heightMultiplier; // Effects the Amplitude of the Perlin Noise so makes the high point and low points stronger

    public override void OnStartClient()
    {
        base.OnStartClient();
        // Randomize variables on the server, this is to give each player the same terrain 
        if (base.IsServer)
        {
            noiseScale = Random.Range(0.02f, 0.03f);
            xOffset = Random.Range(0, 1000);
            zOffset = Random.Range(0, 1000);
            lacunarity = Random.Range(2f, 2.5f);
            persistence = Random.Range(0.5f, 0.4f);

            float heightMin = -8f;
            float heightMax = 10f;
            do
            {
                heightMultiplier = Random.Range(heightMin, heightMax);
            } while (heightMultiplier > -3.9f && heightMultiplier < 3);
            //Runs the tree spawn script after one second after the terrain is generated (So the trees sqawns properly on the terrain)
            Invoke("SpawnTreesAfterDelay", 1f); 

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

    private void SpawnTreesAfterDelay()
    {
        GetComponent<TreeSpawner>().SpawnTrees(); //Calls the SpawnTree methord from the Treespawner script
        Debug.Log("le tree have arrived");
    }


    // Changes the terrain gradient colour every frame, so when you make changes to the terrain in the inspector the gradient updates correctly 
    private void UpdateMaterialProperties()
    {
        float minTerrainHeight = mesh.bounds.min.y + transform.position.y - 0.1f;
        float maxTerrainHeight = mesh.bounds.max.y + transform.position.y + 0.1f;

        mat.SetTexture("terrainGradient", gradientTexture);
        mat.SetFloat("minTerrainHeight", minTerrainHeight);
        mat.SetFloat("maxTerrainHeight", maxTerrainHeight);
    }

    // Generate gradient texture based on terrain height
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

    // Create the initial empty mesh where the terrain mesh will be stored in
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

    // Generates the terrain Island/Lake
    private void GenerateTerrain()
    {
        // Initialize array to hold vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;

        // Nested Loop that goes through each point to create vertices
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float yPos = 0;

                // for each octave (noise layer) a perlin noise map is created
                for (int y = 0; y < octavesAmount; y++)
                {
                    float frequency = Mathf.Pow(lacunarity, y);
                    float amplitude = Mathf.Pow(persistence, y);
                    yPos += Mathf.PerlinNoise((x + xOffset) * noiseScale * frequency,
                                              (z + zOffset) * noiseScale * frequency) * amplitude;
                }

                // Applies the fallof to the terrain, lowers the yPos around the edges of the terrain

                float falloffValue = falloffMap[x, z];
                yPos -= falloffValue;

                // Apply height multiplier yo yPos 
                yPos *= heightMultiplier;

                //Gives each vertices on the mesh a position in the game so on the X, Z and using the yPos
                vertices[i] = new Vector3(x, yPos, z);
                i++;
            }
        }

        // Initialize list to store triangle indices
        List<int> triangles = new List<int>();
        int vert = 0;

        // Loop through each verticie to generate triangles
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // creates the a square by makeing a lines from each verticie
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

        // Clear existing mesh data so that each time there will be a new mesh
        mesh.Clear();

        // Assign vertices and triangles to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();

        // Recalculate normals for proper lighting
        mesh.RecalculateNormals();

        // Update MeshCollider with new mesh to the collision workes properly everytimes a new terrain is made
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}