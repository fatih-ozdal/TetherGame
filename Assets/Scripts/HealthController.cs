using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform heartsParent;
    [SerializeField] private GameObject heartContainerPrefab;

    private GameObject[] heartContainers;
    private Image[] heartFills;

    private void Start()
    {
        // Auto-find PlayerHealth if not assigned
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found! Make sure there's a PlayerHealth component in the scene.");
            return;
        }

        // Initialize heart containers
        heartContainers = new GameObject[playerHealth.MaxHealth];
        heartFills = new Image[playerHealth.MaxHealth];

        // Subscribe to health changed event
        playerHealth.onHealthChangedCallback += UpdateHeartsHUD;

        // Setup hearts
        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }

    private void OnDestroy()
    {
        // Unsubscribe from event to prevent memory leaks
        if (playerHealth != null)
        {
            playerHealth.onHealthChangedCallback -= UpdateHeartsHUD;
        }
    }

    /*
    void InstantiateHeartContainers()
    {
        for (int i = 0; i < playerHealth.MaxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("Heart_Fill").GetComponent<Image>();
        }
    } 
    */

    void InstantiateHeartContainers()
    {
        Debug.Log("MaxHealth: " + playerHealth.MaxHealth);
        
        for (int i = 0; i < playerHealth.MaxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            
            Transform fillTransform = temp.transform.Find("Heart_Fill");
            Debug.Log("Heart_Fill found: " + (fillTransform != null));
            
            if(fillTransform != null)
            {
                Image img = fillTransform.GetComponent<Image>();
                Debug.Log("Image component found: " + (img != null));
                heartFills[i] = img;
            }
        }
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < playerHealth.MaxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < playerHealth.Health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }

    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}
