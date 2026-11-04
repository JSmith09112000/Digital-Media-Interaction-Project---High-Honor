using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileClickMove : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClickAction;

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
    private Vector3 targetPosition;

    [SerializeField]
    private Tilemap tilemap;

    private Dictionary<Vector3Int, PathNode> nodes = new Dictionary<Vector3Int, PathNode>();

    // Used in highlighting selected tile
    [SerializeField]
    private GameObject hoverIndicator;

    private void Awake()
    {

        mainCamera = Camera.main;

        unitMovement = GetComponent<UnitMovement>();

    }

    private void OnEnable()
    {
        mouseClickAction.Enable();
        mouseClickAction.performed += Move;
    }

    private void OnDisable()
    {
        mouseClickAction.performed -= Move;
        mouseClickAction.Disable();
    }

    private void Update()
    {
        HoverHighlight();
    }

    private void HoverHighlight()
    {
        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hoverIndicator.SetActive(true);
            Vector3 newPosition = hit.collider.transform.position;
            newPosition.y = 2;
            hoverIndicator.transform.position = newPosition;
        }
        else
        {
            hoverIndicator.SetActive(false);
        }
    }

    private void Move(InputAction.CallbackContext context)
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
        {

            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);


            // Makes it so so that players must wait for movement to stop before starting to move again.
            if (movementRoutine != null)
            {
                Debug.Log("movementRoutine is not null");
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
                Debug.Log("path is null");
                return;
            }
            int totalCost = path[path.Count - 1].gCost;

            if (totalCost > unitMovement.movementPoints)
            {
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
        }
        else
        {
            Debug.Log("Raycast hit NOTHING");
        }
    }

    private IEnumerator FollowPath(List<PathNode> path)
    {
        foreach(PathNode node in path)
        {
            Vector3 targetPosition = CellToWorld(node.cellPosition);

            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, playerSpeed * Time.deltaTime);

                yield return null;
            }

            transform.position = targetPosition;

            movementRoutine = null;
        }
    }

    private Vector3 CellToWorld(Vector3Int cell)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(cell);
        worldPosition.y = yOffset;

        return worldPosition;
    }

    /*private IEnumerator PlayerMoveTowards(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            Vector3 destination = Vector3.MoveTowards(
                transform.position,
                target,
                playerSpeed * Time.deltaTime
            );

            transform.position = destination;

            yield return null;
        }

    }*/

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(targetPosition, 1);
    }
}
