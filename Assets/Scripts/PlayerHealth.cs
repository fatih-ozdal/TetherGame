using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float hitFlashSpeed = 10f;
    private bool isInvincible = false;
    private bool isDead = false;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float damageFlashDuration = 0.1f;

    [Header("Particle Effects (Optional)")]
    [SerializeField] private GameObject bloodParticlesPrefab;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Hit Stop (Optional)")]
    [SerializeField] private bool enableHitStop = false;
    [SerializeField] private float hitStopTimeScale = 0.1f;
    [SerializeField] private float hitStopDuration = 0.035f;
    private bool restoreTime = false;
    private float restoreTimeSpeed = 10f;

    // Event system for UI updates
    public delegate void OnHealthChanged();
    public OnHealthChanged onHealthChangedCallback;

    // Public property for controlled health access
    public int Health
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = Mathf.Clamp(value, 0, maxHealth);

                // Trigger callback for UI updates
                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    public int MaxHealth => maxHealth;
    public bool IsInvincible => isInvincible;
    public bool IsDead => isDead;

    private void Awake()
    {
        // Auto-find SpriteRenderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        Health = maxHealth; // Use property to trigger callback
    }

    private void Update()
    {
        // Flash effect while invincible
        FlashWhileInvincible();

        // Restore time scale after hit stop
        if (enableHitStop)
        {
            RestoreTimeScale();
        }
    }

    public void TakeDamage(int damage)
    {
        // Prevent damage if invincible or dead
        if (isInvincible || isDead)
            return;

        Health -= damage;

        if (Health > 0)
        {
            // Player still alive - apply damage feedback
            StartCoroutine(DamageSequence());
        }
        else
        {
            // Player died
            Die();
        }
    }

    private IEnumerator DamageSequence()
    {
        // Start invincibility
        isInvincible = true;

        // Visual feedback
        FlashHit();

        // Spawn blood particles if assigned
        if (bloodParticlesPrefab != null)
        {
            GameObject bloodParticles = Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(bloodParticles, 1.5f);
        }

        // Optional hit stop effect
        if (enableHitStop)
        {
            HitStopTime(hitStopTimeScale, hitStopDuration);
        }

        // Wait for invincibility duration
        yield return new WaitForSeconds(invincibilityDuration);

        // End invincibility
        isInvincible = false;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Optional hit stop on death
        if (enableHitStop)
        {
            HitStopTime(hitStopTimeScale, 0.35f);
        }

        // Death logic
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // You can add death animation trigger here if you have an Animator
        // GetComponent<Animator>()?.SetTrigger("Death");

        // Stop player movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Wait a bit for death animation (optional)
        yield return new WaitForSeconds(0.5f);

        // Respawn or reload scene
        Respawn();
    }

    private void Respawn()
    {
        if (respawnPoint != null)
        {
            // Respawn at checkpoint
            transform.position = respawnPoint.position;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // Reset health and state
            Health = maxHealth;
            isDead = false;
            isInvincible = false;
        }
        else
        {
            // Reload scene if no respawn point
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Visual Effects
    private void FlashHit()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(HitFlashCoroutine());
        }
    }

    private IEnumerator HitFlashCoroutine()
    {
        Color originalColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < damageFlashDuration)
        {
            spriteRenderer.color = Color.Lerp(originalColor, damageFlashColor, elapsedTime / damageFlashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;
    }

    private void FlashWhileInvincible()
    {
        if (spriteRenderer != null && isInvincible && !isDead)
        {
            // Flicker effect while invincible
            spriteRenderer.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f));
        }
        else if (spriteRenderer != null && !isInvincible)
        {
            // Reset to white when not invincible
            spriteRenderer.color = Color.white;
        }
    }

    // Hit Stop System (optional cinematic effect)
    private void HitStopTime(float newTimeScale, float delay)
    {
        if (delay > 0)
        {
            StopCoroutine(nameof(StartTimeAgain));
            StartCoroutine(StartTimeAgain(delay));
        }
        else
        {
            restoreTime = true;
        }

        Time.timeScale = newTimeScale;
    }

    private IEnumerator StartTimeAgain(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        restoreTime = true;
    }

    private void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    // Utility methods
    public void Heal(int amount)
    {
        if (isDead)
            return;

        Health = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        Health = Mathf.Min(currentHealth, maxHealth);
    }

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
}
