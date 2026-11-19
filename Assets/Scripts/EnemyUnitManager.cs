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

    private bool enemyPerformingTurn = false;

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
        if (turnTracker.enemyTurn && !enemyPerformingTurn)
        {
            StartCoroutine(performEnemyTurn());
        }

        enemies.RemoveAll(item => item == null);
    }

    private IEnumerator performEnemyTurn()
    {
        enemyPerformingTurn = true;
        foreach (EnemyAI enemy in enemies)
        {
           yield return StartCoroutine(enemy.runEnemyAI());
            //yield return new WaitForSeconds(1f);
        }
        turnTracker.endEnemyTurn();
        enemyPerformingTurn = false;
    } 
}
