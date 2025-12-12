using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float airControlMultiplier = 0.7f;
    
    [Header("Jump Improvements")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckOffset = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Slide")]
    [SerializeField] private bool enableWallSlide = true;
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallSlideSpeed = 4f;
    [SerializeField] private float wallCheckOffset = 0.2f;

    [Header("References")]
    [SerializeField] private Tutorial_GrapplingRope grappleRope;
    [SerializeField] private Tutorial_GrapplingGun grappleGun; 

    private bool wasGrappling = false;
    private bool preserveMomentum = false;

    private Rigidbody2D rb;
    private BoxCollider2D col;

    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;
    
    // Timers
    private Timer coyoteTimer;
    private Timer jumpBufferTimer;
    
    // Wall slide
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isWallSliding;

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        Vector2 worldSize = Vector2.Scale(col.size, transform.lossyScale);
        float halfWidth = worldSize.x / 2f;
        float halfHeight = worldSize.y / 2f;

        groundCheck.localPosition = new Vector3(0f, -halfHeight - groundCheckOffset, 0f);
        wallCheckLeft.localPosition = new Vector3(-halfWidth - wallCheckOffset, 0f, 0f);
        wallCheckRight.localPosition = new Vector3(halfWidth + wallCheckOffset, 0f, 0f);
        
        // Initialize timers
        coyoteTimer = new Timer(coyoteTime);
        jumpBufferTimer = new Timer(jumpBufferTime);
    }
    
    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Coyote Timer
        if (isGrounded)
        {
            coyoteTimer.Reset();
        }
        else
        {
            coyoteTimer.Tick(Time.deltaTime);
        }

        // Jump Buffer Timer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer.Reset();
        }
        else
        {
            jumpBufferTimer.Tick(Time.deltaTime);
        }

        // Jump Logic - buffer running ve coyote running ise
        if (jumpBufferTimer.IsRunning && coyoteTimer.IsRunning)
        {
            Jump();
            jumpBufferTimer.ForceExpire();
        }

        // Variable Jump Height
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            coyoteTimer.ForceExpire();
        }

        // Wall Slide Check
        if (enableWallSlide)
        {
            CheckWallSlide();
        }
    }

    private void FixedUpdate()
    {
        bool currentlyGrappling = (grappleRope != null && grappleRope.isGrappling);

        if (!wasGrappling && currentlyGrappling)
        {
            //Debug.Log("Started new grapple - disabling momentum preservation");
            preserveMomentum = false; // YENİ - grapple başladı, momentum koruma kapat
        }
        
        if (wasGrappling && !currentlyGrappling)
        {
            //Debug.Log($"Released grapple! Momentum: {rb.linearVelocity}");
            preserveMomentum = true; // Momentum koruma başlat
        }
        
        wasGrappling = currentlyGrappling;
        
        // MOMENTUM KORUMA - Havadayken koru
        if (preserveMomentum && !isGrounded)
        {
            //Debug.Log($"Preserving momentum (airborne): {rb.linearVelocity}");
            
            // Ground check
            wasGrounded = isGrounded;
            if (groundCheck != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckOffset, groundLayer);
            }
            
            // Landing check
            if (isGrounded && !wasGrounded)
            {
                OnLanding();
            }
            
            return; // Velocity'ye dokunma
        }
        
        // YERE DEĞDİ - momentum koruma bitir
        if (preserveMomentum && isGrounded)
        {
            //Debug.Log("Landed - momentum preservation ended");
            preserveMomentum = false;
        }
        
        // LAUNCH MODE
        if (grappleRope != null && grappleRope.isGrappling && 
            grappleGun != null && grappleGun.isLaunchMode)
        {
            wasGrounded = isGrounded;
            if (groundCheck != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckOffset, groundLayer);
            }
            return;
        }
        
        // SWING MODE
        if (grappleRope != null && grappleRope.isGrappling && !grappleGun.isLaunchMode)
        {
            float maxSwingSpeed = 15f;
            if (rb.linearVelocity.magnitude > maxSwingSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSwingSpeed;
            }
            
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                Vector2 moveForce = Vector2.right * moveInput * moveSpeed * 0.5f;
                rb.AddForce(moveForce, ForceMode2D.Force);
            }
            
            wasGrounded = isGrounded;
            if (groundCheck != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckOffset, groundLayer);
            }
            
            return;
        }
        
        // NORMAL MOVEMENT
        float targetSpeed = moveInput * moveSpeed;
        
        if (!isGrounded)
        {
            targetSpeed *= airControlMultiplier;
        }

        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }

        wasGrounded = isGrounded;
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckOffset, groundLayer);
        }

        if (isGrounded && !wasGrounded)
        {
            OnLanding();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckWallSlide()
    {
        if (wallCheckLeft == null || wallCheckRight == null) return;

        Vector2 boxSize = new Vector2(wallCheckOffset, col.size.y * 0.8f);

        // Sol duvar
        RaycastHit2D hitLeft = Physics2D.BoxCast(
            wallCheckLeft.position,
            boxSize,
            0f,
            Vector2.left,
            wallCheckOffset,
            groundLayer
        );
        isTouchingWallLeft = hitLeft.collider != null;

        // Sağ duvar
        RaycastHit2D hitRight = Physics2D.BoxCast(
            wallCheckRight.position,
            boxSize,
            0f,
            Vector2.right,
            wallCheckOffset,
            groundLayer
        );
        isTouchingWallRight = hitRight.collider != null;

        // Wall slide koşulları
        bool movingLeft = moveInput < -0.1f;
        bool movingRight = moveInput > 0.1f;

        if (!isGrounded && ((isTouchingWallLeft && movingLeft) || (isTouchingWallRight && movingRight)))
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void OnLanding()
    {
        Debug.Log("Landed!");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckOffset);
        }

        if (enableWallSlide && col != null)
        {
            if (wallCheckLeft != null)
            {
                Gizmos.color = isTouchingWallLeft ? Color.red : Color.blue;
                Vector3 topPoint = wallCheckLeft.position + Vector3.up * 0.3f;
                Vector3 midPoint = wallCheckLeft.position;
                Vector3 botPoint = wallCheckLeft.position + Vector3.down * 0.3f;
                
                Gizmos.DrawLine(topPoint, topPoint + Vector3.left * wallCheckOffset);
                Gizmos.DrawLine(midPoint, midPoint + Vector3.left * wallCheckOffset);
                Gizmos.DrawLine(botPoint, botPoint + Vector3.left * wallCheckOffset);
            }

            if (wallCheckRight != null)
            {
                Gizmos.color = isTouchingWallRight ? Color.red : Color.blue;
                Vector3 topPoint = wallCheckRight.position + Vector3.up * 0.3f;
                Vector3 midPoint = wallCheckRight.position;
                Vector3 botPoint = wallCheckRight.position + Vector3.down * 0.3f;
                
                Gizmos.DrawLine(topPoint, topPoint + Vector3.right * wallCheckOffset);
                Gizmos.DrawLine(midPoint, midPoint + Vector3.right * wallCheckOffset);
                Gizmos.DrawLine(botPoint, botPoint + Vector3.right * wallCheckOffset);
            }
        }
    }
}
