using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    
    public int CurrentHealth => currentHealth;
    
    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        // Respawn
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            currentHealth = maxHealth;
        }
        else
        {
            // Scene restart
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
