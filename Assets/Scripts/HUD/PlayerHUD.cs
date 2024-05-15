using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider slider;
    public int currentHealth;
    public int maxHealth = 100;
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        slider.value = currentHealth;
    }
    public void Start()
    {
        slider.maxValue = maxHealth;
        currentHealth = maxHealth;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { TakeDamage(10); }
    }
}
