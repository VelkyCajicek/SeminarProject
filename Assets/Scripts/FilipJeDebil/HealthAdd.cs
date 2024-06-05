using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAdd : MonoBehaviour, IItem
{
    public int Amount = 10;
    public static Action<int> OnHealthCollect;
    
    public void Collect()
    {
        OnHealthCollect.Invoke(Amount);
        Destroy(gameObject);
    }
}
