using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Variables related to movement
    public float jump, horizontal;
    private float speed = 8f;
    private bool isFacingRight = false;
    private Rigidbody2D rb;

    // Variables related to player spawn location
    WorldGeneration worldGeneration;
    [SerializeField] GameObject Terrain;
    
    private void Awake() // Awake functions run first so therefore we get the size of the map before assigning it anywhere
    {
        worldGeneration = Terrain.GetComponent<WorldGeneration>();
    }
    void Start()
    {
        // References Rigidbody to edit the vectors connected to this component
        rb = GetComponent<Rigidbody2D>();

        // Spawn the player in the middle of the map
        float worldSize = worldGeneration.worldSize;
        transform.position = new Vector3(worldSize / 2, worldSize / 2);
    }

    void Update()
    {
        // Locks rotation
        transform.rotation = Quaternion.identity; 
        
        // Allows player to jump
        horizontal = Input.GetAxisRaw("Horizontal"); 
        if (Input.GetButtonDown("Jump")) {rb.AddForce(new Vector2(rb.velocity.x, jump));}
        Flip();
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
}
