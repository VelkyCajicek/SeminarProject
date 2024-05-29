using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : EntityClass
{
    // Variables related to movement
    //private float horizontal;
    public GameObject player;
    public Collider2D playerCollider;
    public float distanceFromPlayer;

    //LootTable
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

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
        /*float overSpeedLimit = Vector2.Distance(Vector2.zero, moveEnemy) / speed;
        if (overSpeedLimit > 1)
        {
            moveEnemy /= overSpeedLimit;
        }*/
        rb.velocity = new Vector2(Math.Min(rb.velocity.x, speed), rb.velocity.y);

        rb.velocity = moveEnemy;

        if (objectCollider.IsTouching(playerCollider) && canAttack())
        {
            attackAnotherEntity(player.GetComponent<EntityClass>());
            //player.GetComponent<Player>().removeHealth(attackStrength);
        }

        Flip();
        //transform.position = Vector2.MoveTowards(transform.position,player.transform.position,speed);

    }
    private void FixedUpdate()
    {
        fixedUpdate();
    }
    public override void die()
    {
        foreach (LootItem lootItem in lootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemToGet);
            }
        }
        Destroy(thisObject);
    }

    void InstantiateLoot(GameObject loot)
    {
        if (loot)
        {
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);

            //droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    public override void updateHealth(){}
}
