using Fusion;
using UnityEngine;

public class XP_System : NetworkBehaviour
{
    [Networked] public int currentXP { get; set; } = 0;
    [Networked] public int xpLevelUp { get; set; } = 100;
    [Networked] public int currentLevel { get; set; } = 1;

    //fighter
    [Networked] public int speed { get; set; } = 5;
    [Networked] public int strength { get; set; } = 5;
    [Networked] public int agility { get; set; } = 100;
    [Networked] public int vitality { get; set; } = 100;

    //gunner
    [Networked] public int magazine_size { get; set; } = 100;
    [Networked] public int reload_speed { get; set; } = 100;

    //multiclass attributes
    [Networked] public int expertise { get; set; } = 100;
    //multiclass attributes
    public string classType;

    PopUpSelection popUp;

    public playerShooter shooter;
    public playerMelee playerMelee;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            EnemyScript.onEnemyKilled += addExperience;
        }
        var popups = FindObjectsByType<PopUpSelection>(FindObjectsSortMode.None);
        foreach (var popup in popups)
        {
            if (popup.popUpType == classType)
            {
                popUp = popup;
            }
            else
            {
                popup.gameObject.SetActive(false);
            }
            if (popup.popUpType == classType)
            {
                popUp = popup;
            }
            else
            {
                popup.gameObject.SetActive(false);
            }
        }
        popUp.xP_System = gameObject.GetComponent<XP_System>();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Object.HasStateAuthority)
        {
            EnemyScript.onEnemyKilled -= addExperience;
        }
    }

    private void addExperience(int xpAmount)
    {
        if (Object.HasStateAuthority)
        {
            currentXP += xpAmount;
            if (currentXP >= xpLevelUp)
            {
                LevelUp();
            }
        }
    }
    private void LevelUp()
    {
        if (Object.HasStateAuthority)
        {
            currentLevel++;
            currentXP -= xpLevelUp;
            xpLevelUp += 100;
        }
        RPC_ShowlvlUpPanel();
        Debug.Log("LEVEL UP!!!!!!!!!!!!!");
    }
    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void RPC_IncreaseStat(string typeOfStat)
    {
        if (Object.HasStateAuthority)
        {
            if (classType == "fighter")
            {
                switch (typeOfStat)
                {
                    case "Speed":
                        speed ++;
                        playerMelee.speed = speed;
                        Debug.Log("Increased speed: " + speed);
                        break;
                    case "Strength":
                        strength ++;
                        playerMelee.strength = strength;
                        Debug.Log("Increased strength: " + strength);
                        break;
                    case "Vitality":
                        vitality ++;
                        playerMelee.vitality = vitality;
                        Debug.Log("Increased vitality: " + vitality);
                        break;
                    case "Agility":
                        agility++;
                        playerMelee.agility = agility;
                        Debug.Log("Increased agility: " + agility);
                        break;
                    case "Expertise":
                        vitality ++;
                        playerMelee.vitality = vitality;
                        Debug.Log("Increased expertise: " + expertise);
                        break;
                }
            }
            if (classType == "Gunner")
            {
                switch (typeOfStat)
                {
                    case "Magazine size":
                        magazine_size++;
                        shooter.magazine_size = magazine_size;
                        Debug.Log("Magazine size: " + magazine_size);
                        break;
                    case "Reload Speed":
                        reload_speed++;
                        shooter.reload_speed = reload_speed;
                        Debug.Log("Increased Reload Speed: " + reload_speed);
                        break;
                    case "Expertise":
                        expertise++;
                        shooter.expertise = expertise;
                        Debug.Log("Increased expertise: " + expertise);
                        break;
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ShowlvlUpPanel()
    {
        if (Object.HasStateAuthority)
        {
            if (popUp != null)
            {
                popUp.showPopUp(currentLevel);
            }
        }
    }
}
