using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAdd : MonoBehaviour, IItem
{
    public int Amount = 10;
    public static Action<int> OnHealthCollect;
    public GameObject Player;
    
    public void Collect()
    {
        if (OnHealthCollect != null) OnHealthCollect.Invoke(Amount);
        else
        {
            Player.GetComponent<Player>().Heal(Amount);
        }
        Destroy(gameObject);
    }
}
