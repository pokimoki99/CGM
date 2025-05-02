using Fusion;
using UnityEngine;

public class XP_System : NetworkBehaviour
{
    [Networked] public int currentXP { get; set; } = 0;
    [Networked] public int xpLevelUp { get; set; } = 100;
    [Networked] public int currentLevel { get; set; } = 1;
    [Networked] public int speed { get; set; } = 5;
    [Networked] public int damage { get; set; } = 5;
    [Networked] public int health { get; set; } = 100;

    PopUpSelection popUp;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            EnemyScript.onEnemyKilled += addExperience;
            UIManager.Singleton.localXP= this;
        }
        popUp = FindFirstObjectByType<PopUpSelection>();
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
            switch (typeOfStat)
            {
                case "Speed":
                    speed += 1;
                    Debug.Log("Increased speed: " + speed);
                    break;
                case "Damage":
                    damage += 2;
                    Debug.Log("Increased damage: " + damage);
                    break;
                case "Health":
                    health += 10;
                    Debug.Log("Increased health: " + health);
                    break;
                    
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
