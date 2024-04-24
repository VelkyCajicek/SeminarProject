using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBackground : MonoBehaviour
{
    public GameObject background;
    public WorldGeneration worldGeneration;
    [SerializeField] GameObject Terrain;

    private void Awake()
    {
        worldGeneration = Terrain.GetComponent<WorldGeneration>();
    }
    void Start()
    {
        
        float worldSize = worldGeneration.worldSize;

        background.transform.position = new Vector3(worldSize / 2, 50);
        background.transform.localScale = new Vector3(worldSize / 10, worldSize / 20);

    }

    
    void Update()
    {
        
    }
}
