using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    // Variables related to movement
    //private float horizontal;
    public GameObject player;
    private float speed = 8f;
    private float distanceFromPlayer;
    private float jumpingPower = 6f;
    //private bool isFacingRight = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public Vector2 spawnPos;

    //public AudioSource footSteps;

    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
        distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
        Vector2 playerDirection = player.transform.position - transform.position;
        //Debug.Log(playerDirection);
        Vector2 moveEnemy;
        if (IsGrounded() && playerDirection.y > 0.1)
        {
            moveEnemy.y = jumpingPower;
        }
        else
        {
            moveEnemy.y = rb.velocity.y;
        }
        moveEnemy.x = (playerDirection.x < speed) ? playerDirection.x : (playerDirection.x < 0) ? -speed:speed;
        rb.velocity = new Vector2(moveEnemy.x, moveEnemy.y);

        //transform.position = Vector2.MoveTowards(transform.position,player.transform.position,speed);

    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
