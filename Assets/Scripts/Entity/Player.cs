using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class Player : EntityClass
{
    //IDataPersistenc
    //EntityClass contains all variables etc.
    private bool isFlying = false;
    private bool isInAir = false;
    public Animator animator;
    public Animation attackAnimation;
    public AudioClip[] missAttackSound;
    public AudioClip[] hitEnemySound;
    private void Awake() // Awake functions run first so therefore we get the size of the map before assigning it anywhere
    {
        GetComponent<Transform>().position = spawnPos;
    }
    void Start()
    {
        // References Rigidbody to edit the vectors connected to this component
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
       // Debug.Log("Cooldown:" + currentAttackCooldown);
        // Locks rotation
        transform.rotation = Quaternion.identity;

        // Deals with all key presses

        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack())
        {
            //TODO
            Debug.Log("ATTACK");
            animator.SetTrigger("attack");
            //attacking = attackLength;
            Enemy[] enemies = entities.GetComponentsInChildren<Enemy>();
            Enemy closestEnemy = getClosestEnemy(enemies);
            if (closestEnemy != null)
            {
                float distToEnemy = Vector2.Distance(transform.position, closestEnemy.transform.position);
                Vector2 dirToEnemy = transform.position - closestEnemy.transform.position;
                bool facingTowardsEnemy = dirToEnemy.x >= 0 != isFacingRight;
                Debug.LogWarning(facingTowardsEnemy);
                if ((distToEnemy <= 4.5 && facingTowardsEnemy) || (distToEnemy <= 1.5 && !facingTowardsEnemy))
                {
                    attackAnotherEntity(closestEnemy);
                    playRandomSound(hitEnemySound);
                }
                else
                {
                    playRandomSound(missAttackSound);
                    currentAttackCooldown = attackCooldown;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlying = !isFlying;
        }
        if (!isFlying)
        {
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
            if (hit.collider != null)
            {
                float distance = Mathf.Abs(hit.point.y - transform.position.y);
                isInAir = distance > 4;
            }
        }
        else //Flying
        {
            if (Input.GetButton("Jump") && Input.GetKey(KeyCode.LeftShift))
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            else if (Input.GetButton("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, 12f);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                rb.velocity = new Vector2(rb.velocity.x, -12f);
            }
        }


        //if (attacking <= 0) Flip();
        Flip();

        // Footsteps
        if (!isInAir)
        {
            animator.enabled = true;
        }
        else
        {
            animator.enabled = false;
        }


        if (rb.velocity.x != 0 && !isInAir)
        {
            if (!footSteps.isPlaying)
            {
                footSteps.Play();
            }
        }
        else
        {
            footSteps.Stop();
        }

    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        fixedUpdate();
    }

    public override void die()
    {
        GetComponent<Transform>().position = spawnPos;
        setHealth(maxHealth);
    }
    public override void updateHealth() 
    {
        thisObject.GetComponent<PlayerHUD>().updateHeartDisplay();
    }
    public Enemy getClosestEnemy(Enemy[] enemies)
    {
        float dist = float.MaxValue;
        Enemy closestEnemy = null;
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < dist)
            {
                closestEnemy = enemy;
                dist = distance;
            }
        }
        return closestEnemy;
    }
}
