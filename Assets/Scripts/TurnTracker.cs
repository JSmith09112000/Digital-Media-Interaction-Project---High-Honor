using UnityEngine;
using UnityEngine.InputSystem;

public class TurnTracker : MonoBehaviour
{
    // Tracks if it's currently the enemy turn;
    public bool enemyTurn = false;

    public InputActionReference turnTestAction;

    void Update()
    {
        if (turnTestAction.action.WasPressedThisFrame())
        {
            Debug.Log("Enemy turn start");
            endPlayerTurn();
        }
    }

    public void endPlayerTurn()
    {
        enemyTurn = true;
    }

    public void endEnemyTurn()
    {
        enemyTurn = false;
    }
}
