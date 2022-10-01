using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerCombat player;
    public static EnemyManager instance;

    private int enemyCount;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        AIAgent[] enemies = FindObjectsOfType<AIAgent>();
        enemyCount = enemies.Length;
    }

    public void OnEnemyDied()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            player.PlayWinAnimation();
        }
    }
}
