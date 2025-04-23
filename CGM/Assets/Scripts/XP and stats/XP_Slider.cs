using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XP_Slider : MonoBehaviour
{
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    private XP_System xp_System;

    private void Start()
    {
         xp_System = FindFirstObjectByType<XP_System>();
        
    }

    private void Update()
    {
        if (xp_System != null)
        {
            xpSlider.maxValue = xp_System.xpLevelUp;
            xpSlider.value = xp_System.currentXP;
            levelText.text = "Level: " + xp_System.currentLevel;
        }
    }
}
