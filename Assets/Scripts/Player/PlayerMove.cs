using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour, IDataPersistence
{
    // Variables related to movement
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = false;
    private bool isFlying = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public Vector2 spawnPos;

    public AudioSource footSteps;
    
    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
    }
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlying = !isFlying;
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

        if(rb.velocity.x != 0 && IsGrounded())
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
        
    }

    private void Flip() // Movement left and right
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
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

    // For future setting up game saves (Video time 17:26, https://www.youtube.com/watch?v=aUi9aijvpgs)
    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }
    public void SaveData(ref GameData data)
    {
        data.playerPosition = this.transform.position;
    }
}
