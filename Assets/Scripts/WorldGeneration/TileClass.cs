using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTileClass", menuName = "Tile Class")]
public class TileClass : ScriptableObject // Monobehaviour scripts are only run when they are placed on an object
{
    public string tileName;
    public Sprite[] tileSprites;
}
