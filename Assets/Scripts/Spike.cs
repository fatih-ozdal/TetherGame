using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Spike touched: {collision.gameObject.name}");
        
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit spike!");
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            else
            {
                Debug.LogError("PlayerHealth component not found!");
            }
        }
    }
}