using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public GameObject player;

    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;
    public GameObject heartDisplay;
    public GameObject playerHUD;
    public GameObject virtualCamera;
    public void Start()
    {
    }
    public void Update()
    {
    }
    public void updateHeartDisplay()
    {
        int maxHealth = player.GetComponent<Player>().maxHealth;
        int currentHealth = player.GetComponent<Player>().currentHealth;
        Component[] heartArray = heartDisplay.GetComponentsInChildren<Image>();
        heartArray = heartArray.OrderBy(c => c.transform.position.x).ToArray();//sorts from left to right on screen

        int halfHeartValue = maxHealth / heartArray.Length/2;
        int i = 0;
        foreach (Image heart in heartArray)
        {
            i++;
            if (currentHealth >= halfHeartValue*2)
            {
                heart.sprite = fullHeart;
                currentHealth -= halfHeartValue*2;
            }
            else if (currentHealth >= halfHeartValue)
            {
                heart.sprite = halfHeart;
                currentHealth -= halfHeartValue;
            }
            else
            {
                heart.sprite = emptyHeart;
            }
            //player.GetComponent<Transform>().position = new Vector2(heart.transform.position.x, heart.transform.position.y);
            //heart.transform.position = new Vector2(heart.transform.position.x, heart.transform.position.y + i);
            //Debug.Log("Counted:" + i);
        }
    }
}
