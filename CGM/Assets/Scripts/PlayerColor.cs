using Fusion;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    public MeshRenderer MeshRenderer;
    public Material p1Material;
    public Material p2Material;

    [Networked, OnChangedRender(nameof(ColorChanged))]
    public Color NetworkedColor { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            RPC_SetMaterial(Runner.LocalPlayer == PlayerRef.None);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetMaterial(bool isPlayerOne)
    {
        GetComponent<Renderer>().material = isPlayerOne ? p1Material : p2Material;
    }

    void Update()
    {
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.E))
        {
            NetworkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
    }
    void ColorChanged()
    {
        MeshRenderer.material.color = NetworkedColor;
    }
}