using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyUnitManager : MonoBehaviour
{
    // A list of all enemies
    public static List<EnemyAI> enemies;

    [SerializeField]
    private TurnTracker turnTracker;

    private void Start()
    {
        enemies = Object.FindObjectsByType<EnemyAI>().ToList<EnemyAI>();
        enemies = enemies.OrderByDescending(enemy => enemy.initiative).ToList<EnemyAI>();
        // Debug code used so we can read the debug log and tell which enemy is moving
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].enemyNumber = i + 1;
        }
    }

    void Update()
    {
        if (turnTracker.enemyTurn)
        {
            StartCoroutine(performEnemyTurn());
            turnTracker.endEnemyTurn();
        }
    }

    private IEnumerator performEnemyTurn()
    {
        foreach (EnemyAI enemy in enemies)
        {
            enemy.runEnemyAI();
            // Waits for 1 second before starting AI for next unit
            yield return new WaitForSeconds(1f);
        }
        
    } 
}
