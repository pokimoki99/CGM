using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class HealthSlider : MonoBehaviour
{
    public GameObject playerPrefab;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    private Health health;

    public HPManager hpManager;

    private void Start()
    {
        
        if (health != null)
        {
            healthSlider.maxValue = health.networkedHealth;
        }

        hpManager = FindFirstObjectByType<HPManager>();

        health = FindFirstObjectByType<Health>();
    }

    private void Update()
    {
        
        if (health != null)
        {
            healthSlider.value = health.networkedHealth;
            healthText.text = "Health: " + health.networkedHealth;

        }

        if (hpManager != null)
        {
            healthSlider.maxValue = 100;
            healthSlider.value = health.networkedHealth;
            healthText.text = "Health: " + health.networkedHealth;
        }
        
    }

    
    public void SetMaxHealth(float networkedHealth)
    {
        healthSlider.maxValue = health.networkedHealth;
        healthSlider.value = health.networkedHealth;
    }

    public void SetHealth(float networkedHealth)
    {
        healthSlider.value = health.networkedHealth;
    }
    
}