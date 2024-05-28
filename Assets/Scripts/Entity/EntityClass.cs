using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using static Unity.VisualScripting.Member;

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


    public int currentHealth;
    public int currentAttackCooldown = 0;
    public int attacking = 0;

    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    public Collider2D objectCollider;
    public GameObject thisObject;
    public GameObject entities;

    public Vector2 spawnPos;

    public AudioSource footSteps;
    public AudioClip[] deathSound;
    public AudioClip[] hurtSound;
    public bool log = false;
    protected static AudioSource otherSounds;
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
            if (attacking == 0 && log) Debug.Log("Finished Attack");
        }
        if (transform.position.y <= -10)
        {
            die();
        }
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
    public void attackAnotherEntity(EntityClass attackedEntity)
    {
        if (log) Debug.Log("atackin");
        attackedEntity.removeHealth(attackStrength);
        currentAttackCooldown = attackCooldown;
        attacking = attackLength;
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
            //UnityEngine.Object.DontDestroyOnLoad(source.gameObject);
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
    public abstract void die();
}
