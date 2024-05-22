using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : EntityClass
{
    // Variables related to movement
    //private float horizontal;
    public GameObject player;
    public Collider2D playerCollider;
    public float distanceFromPlayer;


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
        moveEnemy.x = (playerDirection.x < speed) ? playerDirection.x : (playerDirection.x < 0) ? -speed : speed;
        rb.velocity = new Vector2(moveEnemy.x, moveEnemy.y);

        if (objectCollider.IsTouching(playerCollider) && canAttack())
        {
            attackAnotherEntity(player);
            //player.GetComponent<Player>().removeHealth(attackStrength);
        }

        Flip();
        //transform.position = Vector2.MoveTowards(transform.position,player.transform.position,speed);

        if (transform.position.y <= -10)
        {
            die();
        }
    }
    public override void die()
    {
        Destroy(thisObject);
    }
}
