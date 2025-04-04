using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpSelection : MonoBehaviour
{

    public Button speed, damage, health;
    public TextMeshProUGUI levelupText;

    public XP_System xP_System;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        xP_System = GetComponent<XP_System>();
        if (xP_System != null )
        {
            xP_System = FindFirstObjectByType<XP_System>();
        }
    }
    public void selectSpeed()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Speed");
        }
        closePopUp();
    }
    public void selectDamage()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Damage");
        }
        closePopUp();
    }
    public void selectHealth()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Health");
        }
        closePopUp();
    }

    void closePopUp()
    {
        gameObject.SetActive(false);
    }  
    public void showPopUp(int level)
    {
        levelupText.text = "Level UP! You reached level " + level;
        gameObject.SetActive(true);
    }
}
