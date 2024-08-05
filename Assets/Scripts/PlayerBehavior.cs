using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private LevelManager levelManager;

    void Start()
    {
        currentHealth = maxHealth;
        levelManager = FindObjectOfType<LevelManager>();
    }

    void Update()
    {
        // Player update logic
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Player death logic
        Debug.Log("Player died");
        if (levelManager != null)
        {
            levelManager.LevelLost();
        }
    }
}
