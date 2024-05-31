using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ambient : EntityClass
{
    public float randomMoveChance;
    // Start is called before the first frame update
    void Start()
    {
        ignoreEnemyCollisions();
        ignoreAmbientCollisions();
        ignorePlayerCollisions();
    }

    // Update is called once per frame
    void Update()
    {
        // Locks rotation
        transform.rotation = Quaternion.identity;

        // Deals with all key presses

        horizontal = Input.GetAxisRaw("Horizontal");
        Flip();
    }
    private void FixedUpdate()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);


        if (Random.Range(0, 100f) <= randomMoveChance) randomMove();
        if (distanceFromPlayer >= 100) die();
        if (transform.position.y <= -10) die();
    }
    public void randomMove()
    {
        int dir = (Random.Range(-1f, 1f) >= 0f) ? 1 : -1;
        rb.velocity = new Vector2(dir*speed,jumpingPower);
        Debug.Log("RandomMove");
    }
    public override void die()
    {
        Destroy(thisObject);
    }
    public override void updateHealth() { }
}
