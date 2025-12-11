using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    
    private Vector3 targetA;
    private Vector3 targetB;
    private bool movingToB = true;
    
    private void Start()
    {
        // Pozisyonları başlangıçta kaydet
        if (pointA != null) targetA = pointA.position;
        if (pointB != null) targetB = pointB.position;
        
        // Platform'u başlangıç pozisyonuna taşı
        transform.position = targetA;
    }
    
    private void Update()
    {
        // Hedef noktayı belirle
        Vector3 target = movingToB ? targetB : targetA;
        
        // Önceki pozisyon
        Vector3 previousPosition = transform.position;
        
        // Platforma doğru hareket et
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
        // Hedefe ulaştı mı? (0.1f daha büyük tolerance)
        float distance = Vector3.Distance(transform.position, target);
        
        if (distance < 0.1f)
        {
            movingToB = !movingToB; // Yönü değiştir
            Debug.Log($"Platform reached target! Now moving to: {(movingToB ? "B" : "A")}");
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
    
    private void OnDrawGizmos()
    {
        // Debug için çizgiler
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }
    }
}
