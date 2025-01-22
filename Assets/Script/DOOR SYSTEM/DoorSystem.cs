using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    [SerializeField] private Sprite closedDoorSprite; // รูปประตูปิด
    [SerializeField] private Sprite openDoorSprite; // รูปประตูเปิด
    [SerializeField] private Collider2D physicalCollider; // Collider ป้องกันการเดินผ่าน
    [SerializeField] private Collider2D triggerCollider; // Trigger สำหรับตรวจจับ Player
    [SerializeField] private bool isOpen = false; // สถานะประตู (เริ่มต้นปิด)

    private SpriteRenderer spriteRenderer; // ตัวจัดการ Sprite

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateDoorState();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
        {
            ToggleDoor(); // เปลี่ยนสถานะประตูเมื่อกด E
        }
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen; // เปลี่ยนสถานะประตู
        UpdateDoorState(); // อัปเดตสถานะประตู
    }

    private void UpdateDoorState()
    {
        if (isOpen)
        {
            spriteRenderer.sprite = openDoorSprite; // เปลี่ยนรูปเป็นประตูเปิด
            physicalCollider.enabled = false; // ปิด Collider ป้องกันการเดินผ่าน
        }
        else
        {
            spriteRenderer.sprite = closedDoorSprite; // เปลี่ยนรูปเป็นประตูปิด
            physicalCollider.enabled = true; // เปิด Collider ป้องกันการเดินผ่าน
        }
    }
}