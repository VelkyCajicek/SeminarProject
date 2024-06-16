using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Burst.CompilerServices;
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
    public GameObject gun;
    public GameObject gunProjectile;
    public List<Sprite> gunProjectileLaserSprites = new List<Sprite>();
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

        if (GetComponent<SpriteRenderer>().color != Color.red) movement();
        else
        {
            rb.velocity = new Vector2((Math.Abs(rb.velocity.x) > 0.25f) ? rb.velocity.x + (rb.velocity.x < 0 ? 0.25f : -0.25f) : 0, rb.velocity.y);
        }

        if (objectCollider.IsTouching(playerCollider) && canAttack() && enemyType != "ranged")
        {
            attackAnotherEntity(player.GetComponent<Player>());
            if (animator!=null)
            {
                animator.SetTrigger("attack");
            }
        }
        Flip();
        FixPosition();
        FixMovement();
    }
    private void movement()
    {

        distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
        Vector2 playerDirection = player.transform.position - transform.position;
        Vector2 moveEnemy = rb.velocity;

        moveEnemy.x = (Math.Abs(playerDirection.x) < speed) ? playerDirection.x : (playerDirection.x < 0) ? -speed : speed;
        if (enemyType == "ranged")
        {
            if (distanceFromPlayer <= 14 && distanceFromPlayer >= 8)
            {
                moveEnemy.x = 0;
                horizontal = (Mathf.Abs(playerDirection.x) < 0.001f) ? 0 : (playerDirection.x > 0) ? 1 : -1;
                SpriteRenderer gunSprite = gun.GetComponent<SpriteRenderer>();
                Vector2 playerDirectionFromGun = player.transform.position - gunSprite.transform.position;
                RaycastHit2D hit = Physics2D.Raycast(gunSprite.transform.position, playerDirectionFromGun);
                float distX = Mathf.Abs(hit.point.x - gunSprite.transform.position.x);
                float distY = Mathf.Abs(hit.point.y - gunSprite.transform.position.y);
                float shootRayDist =  Mathf.Sqrt(distX * distX + distY * distY);
                if (hit.collider == null || shootRayDist >= distanceFromPlayer)
                {
                    drawLaser();
                    AimGun();
                    currentAttackCooldown--;
                    if (currentAttackCooldown <= 0)
                    {
                        //Ranged Attack
                        drawLaser();
                        attackAnotherEntity(player.GetComponent<Player>());
                        if (animator != null)
                        {
                            animator.SetTrigger("attack");
                        }
                    }
                }
                else
                {
                    currentAttackCooldown = attackCooldown;
                    gunProjectile.SetActive(false);
                    gun.SetActive(false);
                }
            }
            else
            {
                currentAttackCooldown = attackCooldown;
                gunProjectile.SetActive(false);
                gun.SetActive(false);
            }
            if (distanceFromPlayer <= 8) moveEnemy.x *= -1;
        }

        Vector2 enemyMoveDirLeftRight = new Vector2(moveEnemy.x, 0);
        //
        if (getDistanceRaycast(enemyMoveDirLeftRight) != float.NaN && getDistanceRaycast(enemyMoveDirLeftRight) > 0.05f)
        {
            if (enemyType == "normal" &&  IsGrounded() && getDistanceRaycast(enemyMoveDirLeftRight) < jumpDistFromWall)
            {
                moveEnemy.y = jumpingPower;
            }
            else if ((enemyType == "spider" || enemyType == "ranged") && getDistanceRaycast(enemyMoveDirLeftRight) < jumpDistFromWall)
            {//No Grounded check - for climbing
                moveEnemy.y = jumpingPower;
            }
        }
        if ((enemyType == "spider" || enemyType == "ranged") && getDistanceRaycast(enemyMoveDirLeftRight) > jumpDistFromWall && moveEnemy.y > 0)
        {
            moveEnemy.y = 0;
        }

        

        rb.velocity = moveEnemy;
    }
    private void AimGun()
    {
        gun.SetActive(true);
        if (gun == null) return;
        SpriteRenderer gunSprite = gun.GetComponent<SpriteRenderer>();
        Vector2 playerDirection = player.transform.position - gunSprite.transform.position;
        float rotation = (float)Math.Atan((playerDirection.x > 0 ? playerDirection.y : -playerDirection.y) / Math.Abs(playerDirection.x))/2;
        gunSprite.transform.rotation = new Quaternion(gunSprite.transform.rotation.x, gunSprite.transform.rotation.y, rotation, gunSprite.transform.rotation.w);
        gunSprite.transform.localScale = new Vector2(1f,1f);
    }
    private void drawLaser()
    {
        gunProjectile.SetActive(true);
        gun.SetActive(true);
        if (gunProjectile == null) return;
        SpriteRenderer gunSprite = gun.GetComponent<SpriteRenderer>();
        SpriteRenderer gunLaserSprite = gunProjectile.GetComponent<SpriteRenderer>();
        Vector2 playerDirection = player.transform.position - (gunSprite.transform.position);
        float rotation = (float)Math.Atan((playerDirection.x > 0 ? playerDirection.y : -playerDirection.y) / Math.Abs(playerDirection.x)) / 2;
        gunLaserSprite.transform.rotation = new Quaternion(gunLaserSprite.transform.rotation.x, gunLaserSprite.transform.rotation.y, rotation, gunLaserSprite.transform.rotation.w);

        gunLaserSprite.sprite = gunProjectileLaserSprites[(attacking > 0 ? 0 : 1)];

        float playerDist = Mathf.Sqrt(playerDirection.x * playerDirection.x + playerDirection.y * playerDirection.y);
        //Pos
        gunLaserSprite.transform.position = (player.transform.position + gunSprite.transform.position + new Vector3(0f, 0f))/2;
        gunLaserSprite.transform.localScale = new Vector2(playerDist/50f, gunLaserSprite.transform.localScale.y);
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
