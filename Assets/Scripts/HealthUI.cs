using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    
    private void Update()
    {
        if (playerHealth != null && healthText != null)
        {
            healthText.text = $"Health: {playerHealth.CurrentHealth}";
        }
    }
}