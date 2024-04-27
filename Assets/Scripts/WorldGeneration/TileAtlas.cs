using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Tile Atlas")]
    public TileClass bottomSand;
    public TileClass middleSand;
    public TileClass topSand;
    //public TileClass topStone;
    //public TileClass bottomStone;
}
