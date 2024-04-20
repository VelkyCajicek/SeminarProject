using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;

    // The values defined in this constructor will be the default values
    // The game starts with when there is no data to load

    public GameData()
    {
        playerPosition = Vector3.zero;
    }
}
