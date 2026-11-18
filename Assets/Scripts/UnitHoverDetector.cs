using UnityEngine;
using UnityEngine.InputSystem;

public class UnitHoverDetector : MonoBehaviour
{
    [Header("References for Scripts")]

    public Camera mainCamera;
    public StatsUI statsUI;

    //For which unit the cursor is currently over
    private Stats hoveredUnit;

    void Update()
    {
        if (Mouse.current == null || mainCamera == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        Stats detectedUnit = null;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            detectedUnit = hit.collider.GetComponentInParent<Stats>();
        }

        if (detectedUnit != hoveredUnit)
        {
            hoveredUnit = detectedUnit;

            if (hoveredUnit != null)
            {
                statsUI.ShowStats(hoveredUnit);
            }
            else
            {
                statsUI.HideStats();
            }
        }
    }


}
