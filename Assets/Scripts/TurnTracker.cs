using UnityEngine;
using UnityEngine.InputSystem;

public class TurnTracker : MonoBehaviour
{
    // Tracks if it's currently the enemy turn;
    public bool enemyTurn = false;

    public InputActionReference turnTestAction;

    // Used so that once the enemy turn ends the player will be able to move and attack again
    [SerializeField]
    private PlayerController playerController;

    void Update()
    {
        if (turnTestAction.action.WasPressedThisFrame() && !enemyTurn)
        {
            Debug.Log("Enemy turn start");
            endPlayerTurn();
        }
    }

    public void endPlayerTurn()
    {
        enemyTurn = true;
        playerController.DeselectUnit();
    }

    public void endEnemyTurn()
    {
        enemyTurn = false;
        playerController.RefreshUnits();
    }
}
