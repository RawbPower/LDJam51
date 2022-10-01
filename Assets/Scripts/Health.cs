using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public Action HealthChanged;

    public int maxHealth;
    public int startHealth = -1;

    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        if (startHealth >= 0)
        {
            currentHealth = startHealth;
        }
        HealthChanged?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseHealth(int health)
    {
        currentHealth += health;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        HealthChanged?.Invoke();
    }

    public void ReduceHealth(int health)
    {
        currentHealth -= health;
        currentHealth = Mathf.Max(0, currentHealth);
        HealthChanged?.Invoke();
        if (currentHealth == 0)
        {
            PlayerCombat player = GetComponent<PlayerCombat>();
            if (player != null)
            {
                player.PlayDeathAnimation();
            }
            else
            {
                FindObjectOfType<EnemyManager>().OnEnemyDied();
                Destroy(gameObject);
            }
        }
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}
