using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ambient : EntityClass
{
    public Animator animator;
    public float randomMoveChance;
    private bool isInAir = false;
    public int moveCooldown;
    private int currentMoveCooldown = 0;
    // Start is called before the first frame update
    void Start()
    {
        ignoreEnemyCollisions();
        ignoreAmbientCollisions();
        ignorePlayerCollisions();
    }
    void Update()
    {
        // Locks rotation
        transform.rotation = Quaternion.identity;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            isInAir = distance > 2.5;
        }
        animator.SetBool("isInAir", isInAir);

        horizontal = (Mathf.Abs(rb.velocity.x) < 0.001f) ? 0 : (rb.velocity.x > 0) ? 1 : -1;
        Flip();
    }
    private void FixedUpdate()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (currentMoveCooldown > 0) currentMoveCooldown--;
        if (Random.Range(0, 100f) <= randomMoveChance && !isInAir && currentMoveCooldown == 0) randomMove();
        if (distanceFromPlayer >= 100) die();
        if (transform.position.y <= -10) die();
    }
    public void randomMove()
    {
        currentMoveCooldown = moveCooldown;
        int dir = (Random.Range(-1f, 1f) >= 0f) ? 1 : -1;
        
        rb.velocity = new Vector2(dir*speed,jumpingPower);
        isInAir = true;
        animator.SetTrigger("jump");

        horizontal = Input.GetAxisRaw("Horizontal");
        Flip();
    }
    public override void die()
    {
        Destroy(thisObject);
    }
    public override void updateHealth() { }
    
}
