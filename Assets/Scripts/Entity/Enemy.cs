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
    public Collider2D playerCollider;
    public float distanceFromPlayer;
    public string enemyType;
    public float jumpDistFromWall;
    public Animator animator;
    //LootTable
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    // Start is called before the first frame update
    void Start()
    {
        ignoreEnemyCollisions();
        ignoreAmbientCollisions();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        }
        updateIsInAir();
        transform.rotation = Quaternion.identity;
        horizontal = (Mathf.Abs(rb.velocity.x) < 0.001f) ? 0 : (rb.velocity.x > 0) ? 1 : -1;

        movement();

        if (objectCollider.IsTouching(playerCollider) && canAttack())
        {
            attackAnotherEntity(player.GetComponent<EntityClass>());
            if (animator!=null)
            {
                animator.SetTrigger("attack");
            }
        }

        Flip();
    }
    private void movement()
    {
        distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
        Vector2 playerDirection = player.transform.position - transform.position;
        Vector2 playerDirLeftRight = new Vector2(playerDirection.x, 0);
        //
        Vector2 moveEnemy = rb.velocity;
        if (getDistanceRaycast(playerDirLeftRight) != float.NaN && getDistanceRaycast(playerDirLeftRight) > 0.05f)
        {
            if (enemyType == "normal" &&  IsGrounded() && getDistanceRaycast(playerDirLeftRight) < jumpDistFromWall)
            {
                moveEnemy.y = jumpingPower;
            }
            else if (enemyType == "spider" && getDistanceRaycast(playerDirLeftRight) < jumpDistFromWall)
            {//No Grounded check - for climbing
                moveEnemy.y = jumpingPower;
            }
        }
        if (enemyType == "spider" && getDistanceRaycast(playerDirLeftRight) > jumpDistFromWall && moveEnemy.y > 0)
        {
            moveEnemy.y = 0;
        }

        moveEnemy.x = (playerDirection.x < speed) ? playerDirection.x : (playerDirection.x < 0) ? -speed : speed;

        rb.velocity = moveEnemy;

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
