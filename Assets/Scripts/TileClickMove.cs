using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileClickMove : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClickAction;

    [SerializeField]
    private float playerSpeed = 10f;

    private Camera mainCamera;
    private Coroutine coroutine;

    // This was missing
    private Vector3 targetPosition;

    // Used in highlighting selected tile
    [SerializeField]
    private GameObject hoverIndicator;

    private void Awake()
    {
        mainCamera = Camera.main;
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

        /*if (currentHoveredTile != null)
        {
            currentHoveredTile.material.SetFloat("_HighlightStrength", 0f);
        }*/
    }

    private void Update()
    {
        HoverHighlight();
    }
    /*

    // Method to highlight the selected tile
    private void hoverHighlight()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Renderer hoveredRenderer = hit.collider.GetComponent<Renderer>();


            if (hoveredRenderer != currentHoveredTile)
            {
                // Removes highlight from previously highlighted tile
                if (currentHoveredTile != null)
                {
                    currentHoveredTile.material.SetFloat("_HighlightStrength", 0f);
                }
            }

            currentHoveredTile = hoveredRenderer;

            // Highlights the new tile
            if (currentHoveredTile != null)
            {
                currentHoveredTile.material.SetFloat("_HighlightStrength", 1f);
            }
        }
        // Removes highlight if no tile selected
        else
        {
            if (currentHoveredTile != null)
            {
                currentHoveredTile.material.SetFloat("_HighlightStrength", 0f);
                currentHoveredTile = null;
            }
        }
    }*/

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

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            targetPosition = hit.point;

            Debug.Log("Moving to: " + targetPosition);

            // Example destination
            Vector3 destination = hit.collider.transform.position;
            destination.y = 2.5f;

            coroutine = StartCoroutine(PlayerMoveTowards(destination));
        }
        else
        {
            Debug.Log("Raycast hit NOTHING");
        }
    }

    private IEnumerator PlayerMoveTowards(Vector3 target)
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(targetPosition, 1);
    }
}
