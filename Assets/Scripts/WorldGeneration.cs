using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    
    public Sprite bottomSand;
    public Sprite topSand;
    public Sprite stone;
    
    public int sandLayerHeight = 5;

    public int worldSize = 160;
    public int chunkSize = 16;
    public GameObject[] worldChunks;

    public float noiseFrequency = 0.04f;
    public float seed;
    public float heightMultiplier = 25f;
    public int heightAddition = 25;
    public Texture2D noiseTexture;
    public bool isSolid = true;

    // For biomes
    public Texture2D biomeMap;
    public float biomeFrequency;

    private void Start()
    {
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        CreateChunks();
        GenerateTerrain();
    }
    public void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldSize, worldSize);
        
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * noiseFrequency, (y + seed) * noiseFrequency);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }

        noiseTexture.Apply();
    }
    public void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x++)
        {
            float height = Mathf.PerlinNoise((x + seed) * noiseFrequency, seed * noiseFrequency) * heightMultiplier + heightAddition; // To determine the "steepness of the terrain"

            for (int y = 0; y < height; y++)
            {
                //This simply determines which tile is used based on its position
                Sprite tileSprite = stone;
                
                if (y < height - sandLayerHeight)
                {
                    tileSprite = stone;
                }
                else if(y > height - 1)
                {
                    tileSprite = topSand;
                }
                else
                {
                    tileSprite = bottomSand;
                }
                // If statement for caves
                if (noiseTexture.GetPixel(x, y).r > 0.1f) // Lower this value the lower the chance of caves being generated
                {
                    PlaceTiles(tileSprite, x, y);
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
            newChunk.name = $"Chunk {i.ToString()}";
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }
    public void PlaceTiles(Sprite tileSprite, int x, int y)
    {
        GameObject newTile = new GameObject();

        int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize) * chunkSize);
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[chunkCoord].transform;

        // So player doesnt fall through the floor
        newTile.AddComponent<BoxCollider2D>();
        newTile.GetComponent<BoxCollider2D>().size = Vector2.one;

        // Adds all tiles to game scene
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }
}