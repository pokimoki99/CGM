using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpSelection : MonoBehaviour
{

    public TextMeshProUGUI levelupText;

    public XP_System xP_System;
    public string popUpType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        xP_System = GetComponent<XP_System>();
        if (xP_System != null )
        {
            xP_System = FindFirstObjectByType<XP_System>();
        }
    }
    public void selectStrength()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Strength");
        }
        closePopUp();
    }
    public void selectAgility()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Agility");
        }
        closePopUp();
    }
    public void selectVitality()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Vitality");
        }
        closePopUp();
    }
    public void selectExpertise()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Expertise");
        }
        closePopUp();
    }
    public void selectSpeed()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Speed");
        }
        closePopUp();
    }
    public void selectMagazine_Size()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Magazine Size");
        }
        closePopUp();
    }
    public void selectReload_Speed()
    {
        if ( xP_System != null )
        {
            xP_System.RPC_IncreaseStat("Reload Speed");
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
