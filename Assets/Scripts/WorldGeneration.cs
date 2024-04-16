using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public Sprite tile;
    public int worldSize = 100;
    public float noiseFrequency = 0.04f;
    public float seed;
    public float heightMultiplier = 25f;
    public int heightAddition = 25;
    public Texture2D noiseTexture;
    public bool isSolid = true;

    private void Start()
    {
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture();
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
                // If statement for caves
                if (noiseTexture.GetPixel(x,y).r > 0.1f) // Lower this value the lower the chance of caves being generated
                {
                    GameObject newTile = new GameObject(name = "tile");
                    newTile.transform.parent = this.transform;
                    newTile.AddComponent<SpriteRenderer>();
                    
                    // So player doesnt fall through the floor
                    newTile.AddComponent<BoxCollider2D>();
                    newTile.GetComponent<BoxCollider2D>().size = Vector2.one;

                    newTile.GetComponent<SpriteRenderer>().sprite = tile;
                    newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
                }
            }
        }
    }
}
