using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthSlider : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    private Health health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // set up link to health
    }

    // Update is called once per frame
    void Update()
    {
        //update slider to reflect current health
        if (health != null)
        {
            healthSlider.maxValue = health.maxHealth;
            healthSlider.value = health.networkedHealth;
            healthText.text = "Health: " + health.networkedHealth;
        }
    }
}
