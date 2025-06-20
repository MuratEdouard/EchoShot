using UnityEngine;
using UnityEngine.InputSystem;

public class HoverDetector : MonoBehaviour
{
    private Hoverable currentHover;

    [SerializeField] private MenuManager menuManager; // Drag your manager here in the Inspector

    void Update()
    {
        if (Camera.main == null || Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Hoverable hoverable = hit.collider.GetComponent<Hoverable>();
            if (hoverable != null)
            {
                if (hoverable != currentHover)
                {
                    if (currentHover != null)
                        currentHover.OnHoverExit();

                    currentHover = hoverable;
                    currentHover.OnHoverEnter();
                }

                // 👇 Handle click while hovering
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    menuManager?.OnDroneClicked(currentHover);
                }
            }
            else if (currentHover != null)
            {
                currentHover.OnHoverExit();
                currentHover = null;
            }
        }
        else if (currentHover != null)
        {
            currentHover.OnHoverExit();
            currentHover = null;
        }
    }
}
