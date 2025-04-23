using Fusion;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class playerAttack : NetworkBehaviour
{
    public playerShooter shooter;
    public playerMelee melee;
    public int playerStrength, playerDexterity, playerVitality;

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.Keypad1))
        {
            shooter.characterType = "Shooter";
            melee.characterType = "Shooter";
        }

        if (HasStateAuthority && Input.GetKeyDown(KeyCode.Keypad2))
        {
            melee.characterType = "Melee";
            shooter.characterType = "Melee";
        }
    }
    public int playerStrengthChange(int numberChange)
    {
        return playerStrength += numberChange;
    }
    public int playerDexterityChange(int numberChange)
    {
        return playerDexterity += numberChange;
    }
    public int playerVitalityChange(int numberChange)
    {
        return playerVitality += numberChange;
    }

}
