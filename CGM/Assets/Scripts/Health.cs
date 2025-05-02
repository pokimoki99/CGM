using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{

    [Networked,OnChangedRender(nameof(healthChanged))]
    public float networkedHealth { get; set; } = 100;

    public Image healthBarFill;

    public override void Spawned()
    {
        HPManager.instance.registerPlayer(Object);
        UpdateUI();

        if (HasInputAuthority)
        {
            UIManager.Singleton.localHealth = this;
        }

        //healthBarFill.SetMaxHealth(networkedHealth);
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        HPManager.instance.removePlayer(Object);
    }

    
    void healthChanged()
    {
        UpdateUI();
    }

    
    public void UpdateUI()
    {
        {
            healthBarFill.fillAmount = networkedHealth / 100f;
        }
    }
    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void dealDamageRPC(float damage)
    {
        networkedHealth -= damage;
        networkedHealth = Mathf.Clamp(networkedHealth, 0f, 100f);

    }
    public bool Downed()
    {
        if (networkedHealth <= 0)
        {
            networkedHealth = 0;
            return true;
        }
        return false;
    }
    //implement increasing player health revive, when player gets revived, it takes longer
}
