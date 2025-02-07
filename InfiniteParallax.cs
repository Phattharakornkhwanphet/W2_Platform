using UnityEngine;

public class InfiniteParallax : MonoBehaviour
{
    public float parallaxSpeed = 0.5f; // ความเร็วของพื้นหลัง (สามารถเปลี่ยนเป็นลบเพื่อเคลื่อนที่ย้อนกลับได้)
    public float backgroundWidth; // ความกว้างของพื้นหลัง
    public Transform cameraTransform; // ตำแหน่งของกล้อง
    private Vector3 lastCameraPosition; // ตำแหน่งของกล้องก่อนหน้า

    void Start()
    {
        // รับตำแหน่งของกล้อง
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // เก็บตำแหน่งของกล้องในครั้งแรก
        lastCameraPosition = cameraTransform.position;
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x; // หาความกว้างของพื้นหลัง
    }

    void Update()
    {
        // คำนวณการเคลื่อนที่ของกล้อง
        float deltaMovement = cameraTransform.position.x - lastCameraPosition.x;

        // เคลื่อนที่พื้นหลังไปในทิศทางที่กล้องเคลื่อนที่
        transform.position += Vector3.left * deltaMovement * parallaxSpeed;

        // ตรวจสอบว่าพื้นหลังเคลื่อนที่เกินขอบไปทางซ้ายหรือขวาหรือยัง
        if (cameraTransform.position.x - transform.position.x > backgroundWidth || transform.position.x - cameraTransform.position.x > backgroundWidth)
        {
            // รีเซ็ตตำแหน่งพื้นหลังไปอีกฝั่ง
            transform.position = new Vector3(cameraTransform.position.x + Mathf.Sign(deltaMovement) * backgroundWidth, transform.position.y, transform.position.z);
        }

        // เก็บตำแหน่งของกล้องในครั้งล่าสุด
        lastCameraPosition = cameraTransform.position;
    }
}
