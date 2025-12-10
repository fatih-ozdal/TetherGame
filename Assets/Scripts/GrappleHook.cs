using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrappleHook : MonoBehaviour
{
    [Header("Grapple Settings")]
    [SerializeField] private float maxGrappleDistance = 15f;
    [SerializeField] private LayerMask grappleLayer;
    
    [Header("References")]
    [SerializeField] private LineRenderer ropeRenderer;

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private Camera mainCamera;

    private bool isGrappling;
    private Vector2 grapplePoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        if (ropeRenderer != null)
        {
            ropeRenderer.positionCount = 2; // 2 nokta: player -> grapple point
            ropeRenderer.enabled = false; // Başlangıçta kapalı
        }
    }
    
    private void Update()
    {
        // Input handling
        if (Input.GetMouseButtonDown(0)) // Sol tık
        {
            if (!isGrappling)
            {
                StartGrapple();
            }
        }
        
        if (Input.GetMouseButtonUp(0)) // Sol tık bırak
        {
            if (isGrappling)
            {
                StopGrapple();
            }
        }
        
        // Rope visual'ı güncelle
        if (isGrappling)
        {
            UpdateRopeVisual();
        }
    }

    private void StartGrapple()
    {
        // Mouse pozisyonunu world space'e çevir
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // Player'dan mouse'a doğru raycast at
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxGrappleDistance, grappleLayer);
        
        // Debug için ray çiz
        Debug.DrawRay(transform.position, direction * maxGrappleDistance, Color.red, 1f);
        
        // Grapple point bulunduysa
        if (hit.collider != null)
        {
            grapplePoint = hit.point;
            isGrappling = true;
            
            // DistanceJoint2D ekle
            joint = gameObject.AddComponent<DistanceJoint2D>();
            joint.autoConfigureDistance = false;
            joint.connectedAnchor = grapplePoint;
            joint.distance = Vector2.Distance(transform.position, grapplePoint);
            joint.enableCollision = true;
            
            // Rope'u görünür yap
            if (ropeRenderer != null)
            {
                ropeRenderer.enabled = true;
            }
            
            Debug.Log($"Grappled to: {grapplePoint}");
        }
        else
        {
            Debug.Log("No grapple point found!");
        }
    }

    private void StopGrapple()
    {
        isGrappling = false;
        
        // Joint'i yok et
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
        
        // Rope'u gizle
        if (ropeRenderer != null)
        {
            ropeRenderer.enabled = false;
        }
    }

    private void UpdateRopeVisual()
    {
        if (ropeRenderer != null && isGrappling)
        {
            // Rope'un başlangıcı: player pozisyonu
            ropeRenderer.SetPosition(0, transform.position);
            
            // Rope'un bitişi: grapple point
            ropeRenderer.SetPosition(1, grapplePoint);
        }
    }
}
