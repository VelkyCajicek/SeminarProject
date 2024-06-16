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
    public GameObject Terrain;
    private bool isFlying = false;
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
        HealthAdd.OnHealthCollect += Heal;
    }

    void Update()
    {
        Debug.DrawRay(spawnPos, new Vector2(0, 10), Color.red);
        animator.SetBool("isInAir",isInAir);
        // Locks rotation
        transform.rotation = Quaternion.identity;

        // Deals with all key presses

        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack())
        {
            animator.StopPlayback();
            animator.SetTrigger("attack");
            Enemy[] enemies = entities.GetComponentsInChildren<Enemy>();
            Enemy closestEnemy = getClosestEnemy(enemies);
            if (closestEnemy != null)
            {
                float distToEnemy = Vector2.Distance(transform.position, closestEnemy.transform.position);
                Vector2 dirToEnemy = transform.position - closestEnemy.transform.position;
                bool facingTowardsEnemy = dirToEnemy.x >= 0 != isFacingRight;
                if ((distToEnemy <= (this.objectCollider.bounds.size.x+closestEnemy.objectCollider.bounds.size.x) && facingTowardsEnemy) || (distToEnemy <= 1.5 && !facingTowardsEnemy))
                {
                    if (closestEnemy.currentHealth <= attackStrength)
                    {
                        Waves waves = Terrain.GetComponent<Waves>();
                        waves.enemyKilledByPlayer();
                    }
                    attackAnotherEntity(closestEnemy);
                    playRandomSound(hitEnemySound);
                }
            }
            if (attacking <= 0)
            {
                playRandomSound(missAttackSound);
                currentAttackCooldown = attackCooldown;
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Waves waves = Terrain.GetComponent<Waves>();
            waves.enemyKilledByPlayer();
            isFlying = !isFlying;
        }
        if (!isFlying)
        {
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                animator.SetTrigger("jump");
                jumping = true;
            }
            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                jumping = true;
            }
            if (rb.velocity.y < 0 && !isInAir) jumping = false;
            updateIsInAir();
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


        if (attacking <= 0) Flip();
        //Flip();

        // Footsteps
        /*if (!isInAir)
        {
            animator.enabled = true;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            animator.enabled = false;
        }
        */


        if (rb.velocity.x != 0 && IsGrounded())
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

        FixPosition();
        FixMovement();
    }
    private void FixedUpdate()
    {

        rb.velocity = new Vector2(((GetComponent<SpriteRenderer>().color == Color.red && horizontal == 0) ? ((Math.Abs(rb.velocity.x) > 0.25f) ? rb.velocity.x +(rb.velocity.x < 0? 0.25f:-0.25f) : 0) : horizontal * speed), rb.velocity.y);
        FixMovement();
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
