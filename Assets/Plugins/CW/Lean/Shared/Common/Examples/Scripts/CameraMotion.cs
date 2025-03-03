using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    [SerializeField] private float _panSpeed = 20f;
    [SerializeField] private float _panBorderThickness = 10f;
    [SerializeField] private Vector2 _borders;

    private void Update()
    {
        HandleCameraMovement();       
    }

    private void HandleCameraMovement()
    {
        Vector3 pos = transform.localPosition;
        pos += GetKeyboardMovementInput() + GetMousePanInput();
        pos = ClampPosition(pos);
        transform.localPosition = pos;
    }

    private Vector3 GetKeyboardMovementInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * _panSpeed * Time.deltaTime;
        return movement;
    }

    private Vector3 GetMousePanInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 movement = Vector3.zero;

        if (IsMouseAtScreenEdge(mousePosition))
        {
            movement.x = GetHorizontalMouseMovement(mousePosition);
            movement.z = GetVerticalMouseMovement(mousePosition);
        }

        return movement * _panSpeed * Time.deltaTime;
    }

    private bool IsMouseAtScreenEdge(Vector3 mousePosition)
    {
        return mousePosition.y >= Screen.height - _panBorderThickness ||
               mousePosition.y <= _panBorderThickness ||
               mousePosition.x <= _panBorderThickness ||
               mousePosition.x >= Screen.width - _panBorderThickness;
    }

    private float GetHorizontalMouseMovement(Vector3 mousePosition)
    {
        if (mousePosition.x <= _panBorderThickness)
            return -1f;
        else if (mousePosition.x >= Screen.width - _panBorderThickness)
            return 1f;
        else
            return 0f;
    }

    private float GetVerticalMouseMovement(Vector3 mousePosition)
    {
        if (mousePosition.y <= _panBorderThickness)
            return -1f;
        if (mousePosition.y >= Screen.height - _panBorderThickness)
            return 1f;
        return 0f;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, -_borders.x, _borders.x);
        position.z = Mathf.Clamp(position.z, -_borders.y, _borders.y);
        return position;
    }

   
}
