using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;

public class WorldGeneration : MonoBehaviour
{
    [Header("Instances of objects")]
    public Vector2 spawnPos;
    public ParticleSystem spiceEffect;
    public GameObject Player;
    public GameObject EnemyNormal;
    public GameObject EnemySpider;
    public GameObject EnemyHarkonnen;
    public GameObject Ambient;
    public GameObject Entities;

    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;

    [Header("Generation Settings")]
    public int worldSize = 160;
    public int chunkSize = 16;
    public float height;
    public float seed;
    private GameObject[] worldChunks;
    public int sandLayerHeight = 5;
    public bool isSolid = true;

    [Header("Terrain Generation")]
    public int heightAddition = 25;
    public bool generateCaves = true;
    public float surfaceValue = 0.2f; // Lower this value the lower the chance of caves being generated
    public Texture2D caveNoiseTexture;

    [Header("Biomes settings")]
    public Texture2D biomeMap;
    public float biomeFrequency;
    public Gradient biomeColorsGradient;

    [Header("Biomes")]
    public BiomeClass[] biomes;
    public Color[] biomeColors;
    private BiomeClass curBiome;

    private void OnValidate()
    {
        DrawTextures();
    }
    private void Start()
    {
        // Terrain spawn
        seed = UnityEngine.Random.Range(-10000, 10000);
        //seed = 8956;
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        DrawTextures();

        CreateChunks();
        GenerateTerrain();

        // Player spawn
        spawnPlayer();

        spawnAmbient();

        // Particle effects
        spiceEffect.transform.position = new Vector3(0, worldSize / 4);
        spiceEffect.transform.localScale = new Vector3(5f, 0.4f, worldSize / 8);
    }
    public void GenerateNoiseTexture(float frequency, Texture2D noiseTexture)
    {
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }
        noiseTexture.Apply();
    }
    public void GenerateTerrain()
    {
        Sprite[] tileSprites;
        for (int x = 0; x < worldSize; x++)
        {
            curBiome = GetCurrentBiome(x, 0);
            // To determine the "steepness of the terrain"
            height = Mathf.PerlinNoise((x + seed) * curBiome.caveFrequency, seed * curBiome.caveFrequency) * curBiome.heightMultiplier + heightAddition; // Terrain frequency was here, instead of caveFrequency

            if (x == worldSize / 2) // Player spawn
            {
                spawnPos = new Vector2(x, height+2);
            }
            for (int y = 0; y < height; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                if (y < height - sandLayerHeight)
                {
                    tileSprites = curBiome.tileAtlas.bottomSand.tileSprites;
                }
                else if (y > height - 1)
                {

                    tileSprites = curBiome.tileAtlas.topSand.tileSprites;
                }
                else
                {
                    tileSprites = curBiome.tileAtlas.middleSand.tileSprites;
                }
                // If statement for caves
                if (generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(x, y).r > surfaceValue)
                    {
                        PlaceTiles(tileSprites, x, y);
                    }
                }
                else
                {
                    PlaceTiles(tileSprites, x, y);
                }
            }
        }
    }
    public void CreateChunks()
    {
        // Using an array it groups up tiles based on the parameter "chunksize"
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = $"Chunk {i}";
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }
    public BiomeClass GetCurrentBiome(int x, int y)
    {
        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].biomeCol == biomeMap.GetPixel(x, y))
            {
                return biomes[i];
            }
        }
        return curBiome;
    }
    public void DrawTextures()
    {
        biomeMap = new Texture2D(worldSize, worldSize);
        DrawBiomeTexture();
        for (int i = 0; i < biomes.Length; i++)
        {
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            GenerateNoiseTexture(biomes[i].caveFrequency, biomes[i].caveNoiseTexture);
        }
    }
    public void DrawBiomeTexture()
    {
        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                Color col = biomeColorsGradient.Evaluate(v);
                biomeMap.SetPixel(x, y, col);
            }
        }
        biomeMap.Apply();
    }
    public void PlaceTiles(Sprite[] tileSprites, int x, int y)
    {
        GameObject newTile = new GameObject();

        int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize) * chunkSize);
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[chunkCoord].transform;

        // So player doesnt fall through the floor
        newTile.AddComponent<BoxCollider2D>();
        newTile.GetComponent<BoxCollider2D>().size = Vector2.one;

        // Adds ground layer to tiles so player can jump from them
        newTile.layer = LayerMask.NameToLayer("Ground");

        // Adds all tiles to game scene
        newTile.AddComponent<SpriteRenderer>();

        int spriteIndex = UnityEngine.Random.Range(0, tileSprites.Length);
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteIndex];

        newTile.name = tileSprites[0].name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }
    public void spawnAmbient()
    {
        GameObject go = GameObject.Instantiate(Ambient);
        Ambient ambientScript = go.GetComponent<Ambient>();
        go.SetActive(true);
        ambientScript.thisObject = go;
        ambientScript.transform.parent = Entities.transform;
        ambientScript.spawnPos = spawnPos;
        ambientScript.Spawn();
    }
    public void spawnPlayer()
    {
        Player playerScript = Player.GetComponent<Player>();
        playerScript.thisObject = Player;
        playerScript.spawnPos = spawnPos;
        playerScript.Spawn();
    }

}