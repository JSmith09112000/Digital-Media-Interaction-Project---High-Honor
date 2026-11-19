using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{

    [SerializeField]
    private float playerSpeed = 10f;

    [SerializeField]
    private Grid grid;

    // An offset applied to the y Axis so models don't clip into the terrain or float above it
    [SerializeField]
    private float yOffset = 2.5f;

    [SerializeField]
    private Pathfinder pathfinder;

    private Camera mainCamera;
    private Coroutine movementRoutine;

    private UnitMovement unitMovement;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private UnitTracker unitTracker;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private BattleManager battleManager;

    private Dictionary<Vector3Int, PathNode> nodes = new Dictionary<Vector3Int, PathNode>();
    public bool hasMoved = false;
    public bool hasAttacked = false;

    private Stats stats;

    private void Awake()
    {

        mainCamera = Camera.main;

        unitMovement = GetComponent<UnitMovement>();
        stats = GetComponent<Stats>();

    }

    public void Move()
    {

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
        {

            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);


            // Makes it so so that players must wait for movement to stop before starting to move again.
            if (movementRoutine != null)
            {
                Debug.Log("movementRoutine is not null");
                playerController.unitPerfomingAction = false;
                return;
            }
            if (hasMoved)
            {
                Debug.Log("Unit has already moved");
                playerController.unitPerfomingAction = false;
                return;
            }

            Terrain terrain = hit.collider.GetComponent<Terrain>();

            if (terrain == null)
            {
                return;
            }

            Vector3Int startCell = grid.WorldToCell(transform.position);
            Debug.Log($"{startCell}");

            Vector3Int targetCell = grid.WorldToCell(terrain.transform.position);
            Debug.Log($"{targetCell}");



            List<PathNode> path = pathfinder.FindPath(startCell, targetCell, unitMovement);

            if (path == null)
            {
                playerController.unitPerfomingAction = false;
                Debug.Log("path is null");
                return;
            }
            int totalCost = path[path.Count - 1].gCost;

            if (totalCost > unitMovement.movementPoints)
            {
                playerController.unitPerfomingAction = false;
                Debug.Log($"Path costs {totalCost}. Unit only has {unitMovement.movementPoints}");
                return;
            }

            movementRoutine = StartCoroutine(FollowPath(path));

            /*targetPosition = hit.point;

            Debug.Log("Moving to: " + targetPosition);

            // Example destination
            Vector3 destination = hit.collider.transform.position;
            destination.y = 2.5f;

            coroutine = StartCoroutine(PlayerMoveTowards(destination));*/

            playerController.DeselectUnit();
            hasMoved = true;
        }
        else
        {
            playerController.unitPerfomingAction = false;
            Debug.Log("Raycast hit NOTHING");
        }
    }

    public void Attack()
    {

        // Makes it so so that players must wait for movement to stop before they can attack.
        if (movementRoutine != null)
        {
            Debug.Log("movementRoutine is not null");
            playerController.unitPerfomingAction = false;
            return;
        }

        if (hasAttacked)
        {
            Debug.Log("Unit has already attacked.");
            playerController.unitPerfomingAction = false;
            return;
        }
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();

            if (terrain == null)
            {
                playerController.unitPerfomingAction = false;
                return;
            }

            Vector3Int targetCell = grid.WorldToCell(terrain.transform.position);
            Debug.Log($"{targetCell}");

            if (!TargetInRange(targetCell, stats.minRange, stats.maxRange))
            {
                Debug.Log("No enemy in range.");
                playerController.unitPerfomingAction = false;
                return;
            }

            if (unitTracker.HasUnit(targetCell))
            {
                if (unitTracker.GetUnit(targetCell).TryGetComponent<EnemyAI>(out _))
                {
                    GameObject enemy = unitTracker.GetUnit(targetCell).gameObject;
                    Debug.Log(enemy);
                    battleManager.Battle(this.GameObject(), enemy);
                }
            }
            else
            {
                playerController.unitPerfomingAction = false;
                return;
            }
            playerController.unitPerfomingAction = false;
            hasAttacked = true;
            // Attacking also makes it so a unit can't move. (The idea being that attacking has tired the unit out and they need a short rest.)
            hasMoved = true;
        }
        else
        {
            Debug.Log("Raycast hit NOTHING");
            playerController.unitPerfomingAction = false;
        }
    }

    private IEnumerator FollowPath(List<PathNode> path)
    {

        unitTracker.UnregisterUnit(grid.WorldToCell(transform.position));

        foreach (PathNode node in path)
        {
            Vector3 targetPosition = CellToWorld(node.cellPosition);

            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, playerSpeed * Time.deltaTime);

                yield return null;
            }

            transform.position = targetPosition;
        }

        unitTracker.RegisterUnit(grid.WorldToCell(transform.position), unitMovement);

        playerController.unitPerfomingAction = false;
        movementRoutine = null;
    }

    private Vector3 CellToWorld(Vector3Int cell)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(cell);
        worldPosition.y = yOffset;

        return worldPosition;
    }

    private bool TargetInRange(Vector3Int targetCell, int minRange, int maxRange)
    {
        Vector3Int playerCell = grid.WorldToCell(transform.position);
        int distance = Mathf.Abs(playerCell.x - targetCell.x) + Mathf.Abs(playerCell.y - targetCell.y);

        Debug.Log("Distance: " + distance);

        return distance >= minRange && distance <= maxRange;
    }
}
