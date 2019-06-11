using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    [SyncVar (hook= "OnChangeHealth") ] public const int  maxHealth = 100;
    public int currentHealth = maxHealth;
    public RectTransform healthBar;

    public void TakeDamage(int amount){
        Debug.Log("damage.");

        if (!isServer)
        {
            return;
        }

        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("DEAD.");
        }

        
    }
    
    void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }
}
