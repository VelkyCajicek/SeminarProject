using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : EntityClass
{
    //IDataPersistenc
    //EntityClass contains all variables etc.
    private bool isFlying = false;
    public Animator animator;


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
        // Locks rotation
        transform.rotation = Quaternion.identity;

        // Allows player to jump

        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlying = !isFlying;
            removeHealth(5);
        }
        if (!isFlying)
        {
            speed = 8f;
            jumpingPower = 16f;
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
        else //Flying
        {
            speed = 24f;
            jumpingPower = 12f;
            if (Input.GetButton("Jump") && Input.GetKey(KeyCode.LeftShift))
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            else if (Input.GetButton("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                rb.velocity = new Vector2(rb.velocity.x, -jumpingPower);
            }
        }


        Flip();

        // Footsteps

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

        if (transform.position.y <= -10)
        {
            die();
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
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

}
