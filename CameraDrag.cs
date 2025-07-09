using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    public Camera cam;
    public float dragSpeed = 0.1f;

    private Vector2 lastTouchPos;
    private bool isDragging = false;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        // ห้ามลากถ้าคลิกอยู่บน UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // เริ่มลาก (Touch)
        if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
        {
            lastTouchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            isDragging = true;
        }

        // เริ่มลาก (Mouse)
        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            lastTouchPos = Mouse.current.position.ReadValue();
            isDragging = true;
        }

        // ระหว่างลาก
        if (isDragging)
        {
            Vector2 currentPos = Touchscreen.current != null
                ? Touchscreen.current.primaryTouch.position.ReadValue()
                : Mouse.current.position.ReadValue();

            Vector2 delta = currentPos - lastTouchPos;

            // แปลงจากจอ → world → กล้องเคลื่อน
            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * dragSpeed * Time.deltaTime;
            cam.transform.Translate(move, Space.World);

            lastTouchPos = currentPos;
        }

        // ปล่อย (Touch หรือ Mouse)
        if (Touchscreen.current?.primaryTouch.press.wasReleasedThisFrame == true ||
            Mouse.current?.leftButton.wasReleasedThisFrame == true)
        {
            isDragging = false;
        }
    }
}
