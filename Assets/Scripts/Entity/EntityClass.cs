using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityClass : MonoBehaviour
{

    public float horizontal;
    public float speed = 8f;
    public float jumpingPower = 16f;
    protected bool isFacingRight = false;
    public int maxHealth = 100;
    public int attackCooldown = 100;
    public int attackStrength = 10;


    protected int currentHealth = 100;
    protected int currentAttackCooldown = 0;

    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    public Collider2D objectCollider;
    public GameObject thisObject;

    public Vector2 spawnPos;

    public AudioSource footSteps;
    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
        currentHealth = maxHealth;
    }
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    public void Flip() // Movement left and right
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
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
    public bool canAttack()
    {
        return currentAttackCooldown <= 0;
    }
    public void attackAnotherEntity(GameObject attackedEntity)
    {
        attackedEntity.GetComponent<Player>().removeHealth(attackStrength);
        currentAttackCooldown = attackCooldown;
    }
    public void removeHealth(int attackStrength)
    {
        currentHealth -= attackStrength;
        if (currentHealth <= 0)
        {
            setHealth(0);
            die();
        }
        Debug.Log("CurrentHealth: " + currentHealth);
    }
    public void setHealth(int health)
    {
        currentHealth = health;
    }
    public abstract void die();
}
