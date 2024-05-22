using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    // Variables related to movement
    //private float horizontal;
    public GameObject player;
    private float speed = 8f;
    private float distanceFromPlayer;
    private float jumpingPower = 6f;
    private bool isFacingRight = false;
    private int attackCooldown = 100;
    public int health = 100;
    public int attackStrength = 10;
    //private bool isFacingRight = false;
    public int currentAttackCooldown = 0;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    public Collider2D objectCollider;
    public Collider2D playerCollider;

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
        if (currentAttackCooldown >= 1) currentAttackCooldown--;
        transform.rotation = Quaternion.identity;
        distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
        Vector2 playerDirection = player.transform.position - transform.position;
        //
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

        if (objectCollider.IsTouching(playerCollider) && currentAttackCooldown <= 0)
        {
            Debug.Log("hitPlayer");
            player.GetComponent<PlayerMove>().removeHealth(attackStrength);
            currentAttackCooldown = attackCooldown;
        }

        Flip();
        //transform.position = Vector2.MoveTowards(transform.position,player.transform.position,speed);

    }
    private void Flip() // Movement left and right
    {
        if (isFacingRight && rb.velocity.x < 0f || !isFacingRight && rb.velocity.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    public void die()
    {

    }
}
