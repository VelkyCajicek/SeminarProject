using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using static Unity.VisualScripting.Member;
using Unity.Burst.CompilerServices;
using static UnityEditor.PlayerSettings;

public abstract class EntityClass : MonoBehaviour
{

    public float horizontal;
    public float speed;
    public float jumpingPower;
    protected bool isFacingRight = false;
    public int maxHealth;
    public int attackCooldown;
    public int attackStrength;
    public int attackLength;
    public bool isInAir = false;
    public float isInAirThreshold;
    


    public int currentHealth;
    public int currentAttackCooldown = 0;
    public int attacking = 0;

    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    public Collider2D objectCollider;
    public GameObject thisObject;
    public GameObject entities;
    public GameObject player;

    public Vector2 spawnPos;

    public AudioSource footSteps;
    public AudioClip[] deathSound;
    public AudioClip[] hurtSound;
    public bool log = false;
    protected static AudioSource otherSounds;
    protected int redForTicks = 0;
    protected bool jumping = false;


    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
        currentHealth = maxHealth;
    }
    protected void fixedUpdate()
    {
        if (currentAttackCooldown > 0) currentAttackCooldown--;
        if (attacking > 0)
        {
            attacking--;
        }
        if (transform.position.y <= -10)
        {
            die();
        }
        if (GetComponent<SpriteRenderer>().color == Color.red)
        {
            redForTicks++;
            if (redForTicks == 25)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                redForTicks = 0;
            }
        }
    }
    public bool IsGrounded()
    {
        //return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return !isInAir;
    }
    public Vector2 getFeetPos()
    {
        Vector2 feetPos = objectCollider.bounds.center;
        feetPos.y -= objectCollider.bounds.extents.y;
        return feetPos;
    }
    public void updateIsInAir()
    {
        Vector2 sendFrom = getFeetPos();
        if (isInAirThreshold == float.NaN) return;
        RaycastHit2D hit = Physics2D.Raycast(sendFrom, -Vector2.up);
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.y - sendFrom.y);
            isInAir = distance > isInAirThreshold;
            //Debug.DrawLine(sendFrom, hit.point, Color.red);
        }
        else
        {
            isInAir = true;
        }
    }
    public float getDistanceRaycast(Vector2 dir)
    {
        Vector2 sendFrom = getFeetPos();
        RaycastHit2D hit = Physics2D.Raycast(sendFrom, dir);
        if (hit.collider != null)
        {
            float distX = Mathf.Abs(hit.point.x - sendFrom.x);
            float distY = Mathf.Abs(hit.point.y - sendFrom.y);
            //Debug.DrawLine(sendFrom, hit.point, Color.green);
            return Mathf.Sqrt(distX*distX + distY*distY);
        }
        return float.NaN;
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
    public void attackAnotherEntity(EntityClass attackedEntity)
    {
        attackedEntity.removeHealth(attackStrength);
        currentAttackCooldown = attackCooldown;
        attacking = attackLength;

        int attackDir = (this.transform.position.x - attackedEntity.transform.position.x > 0) ? -1 : 1;
        attackedEntity.rb.velocity = new Vector2(attackDir*10, 6);
        attackedEntity.GetComponent<SpriteRenderer>().color = Color.red;
    }
    public void removeHealth(int attackStrength)
    {
        currentHealth -= attackStrength;
        if (currentHealth > 0)
        {
            playRandomSound(hurtSound);
        }
        else
        {
            setHealth(0);
            die();
            playRandomSound(deathSound);
        }
        updateHealth();
    }
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        updateHealth();
    }

    
    public void playRandomSound(AudioClip[] sounds)
    {
        if (sounds.Length == 0) return;
        AudioClip audio = sounds[Random.Range(0, sounds.Length)];
        playSound(otherSounds, audio);
    }
    public void playSound(AudioSource source, AudioClip sound, float volume = 1f)
    {
        if (source == null)
        {
            source = thisObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
        }

        source.clip = sound;
        source.volume = volume;
        source.Play();
    }
    public abstract void updateHealth();
    public void setHealth(int health)
    {
        currentHealth = health;
    }
    public void ignoreEnemyCollisions()
    {
        Enemy[] enemies = entities.GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            Collider2D colliderEnemy = enemy.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(colliderEnemy.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
    public void ignorePlayerCollisions()
    {
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
    public void ignoreAmbientCollisions()
    {
        Ambient[] ambients = entities.GetComponentsInChildren<Ambient>();
        foreach (Ambient ambient in ambients)
        {
            Collider2D colliderAmbient = ambient.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(colliderAmbient.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
    public void FixPosition()
    {
        Vector2 posLeft = objectCollider.bounds.min;
        Vector2 posRight = objectCollider.bounds.max;
        posRight = new Vector2(posRight.x, posLeft.y);

        Vector2 hitLeft = Vector2.zero;
        Vector2 hitRight = Vector2.zero;

        RaycastHit2D hit = Physics2D.Raycast(posLeft + new Vector2(0, 1000), -Vector2.up);
        if (hit.collider != null)
        {
            hitLeft = hit.point;
        }
        RaycastHit2D hit2 = Physics2D.Raycast(posRight + new Vector2(0, 1000), -Vector2.up);
        if (hit2.collider != null)
        {
            hitRight = hit2.point;
        }
        const float tpMargain = 0.8f;
        const float tpAmount = 0.2f;
        if (hitLeft.y - posLeft.y >= tpMargain) transform.position += new Vector3(0, tpAmount, 0);
        if (hitRight.y - posRight.y >= tpMargain) transform.position += new Vector3(0, tpAmount, 0);
    }
    public void FixMovement()
    {
        if (jumping) return;
        float initialYVelocity = rb.velocity.y;
        float maxSpeed = speed;
        float currentSpeed = rb.velocity.x + (rb.velocity.y > 0 ? rb.velocity.y*4 : 0);//(float)Math.Sqrt(rb.velocity.x* rb.velocity.x + rb.velocity.y* rb.velocity.y)
        if (currentSpeed <= maxSpeed) return;
        float multiplyBy = maxSpeed / currentSpeed;
        if (maxSpeed == float.NaN || currentSpeed == float.NaN || multiplyBy == float.NaN) return;
        Vector2 newVelocity = new Vector2(rb.velocity.x*multiplyBy, rb.velocity.y * multiplyBy);
        newVelocity.y = initialYVelocity;
        //if (!isInAir && newVelocity.y > (jumpingPower / 2)) newVelocity.y = jumpingPower / 3.5f;
        if (this.log) Debug.Log($"TESTING: {maxSpeed}___{currentSpeed}___{multiplyBy}___{rb.velocity}___{newVelocity}");
        rb.velocity = newVelocity;
    }
    public abstract void die();
}
