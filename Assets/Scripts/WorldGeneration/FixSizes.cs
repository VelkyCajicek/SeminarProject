using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FixSizes : MonoBehaviour
{
    [Header("Player variables")]
    public GameObject Player;
    public GameObject GroundCheckPlayer;
    public BoxCollider2D collider2DPlayer;

    [Header("Enemy variables")]
    public GameObject Enemy;
    public GameObject GroundCheckEnemy;
    public BoxCollider2D collider2DEnemy;

    private void Awake()
    {
        // Player 
        Player.transform.position = new Vector3(0, 0, 0);
        Player.transform.localScale = new Vector3(4, 4);
        collider2DPlayer.size = new Vector3(0.41f, 0.85f);
        collider2DPlayer.offset = new Vector3(0.02f, -0.16f);

        //GroundCheckPlayer.transform.position = new Vector3(0, -0.6f);

        // Enemy
        Enemy.transform.position = new Vector3(6, 0, 0);
        collider2DEnemy.size = new Vector3(2, 3);
        collider2DEnemy.offset = new Vector3(0, 0);
        
        //GroundCheckEnemy.transform.position = new Vector3(0.083f, -1, 44f);
    }
}
