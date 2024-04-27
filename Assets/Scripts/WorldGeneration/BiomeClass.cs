using System.Collections;
using UnityEngine;

[System.Serializable]
public class BiomeClass
{
    public string biomeName;

    public Color biomeCol;

    public TileAtlas tileAtlas;

    [Header("Generation settings")]
    public float caveFrequency;
    public float terrainFrequency;
    public float heightMultiplier;
    public Texture2D caveNoiseTexture;

    [Header("Noise settings")]
    public float surfaceValue;
    public int sandLayerHeight;
    public bool generateCaves = true;
}
