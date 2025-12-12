using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    
    // Kamera sınırları (level dışına çıkmasın)
    [SerializeField] private float minX = 5f;
    [SerializeField] private float maxX = 25f;
    [SerializeField] private float minY = 5f;
    [SerializeField] private float maxY = 25f;
    
    private void LateUpdate()
    {
        if (player == null) return;
        
        Vector3 desiredPosition = player.position + offset;
        
        // Sınırları uygula
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
