using Fusion;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;
    public Transform redTeamStartPos, blueTeamStartPos;
    public HPManager hpManager;

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer || Runner.GameMode == GameMode.Shared)
        {
            if (player == Runner.LocalPlayer)
            {
                Vector3 spawnPosition = PlayerTeamRandom();
                //NetworkObject playerObject = 
                Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            }
        }
    }
    private Vector3 PlayerTeamRandom()
    {
        if (Random.Range(0, 2) == 0)
        {
            return redTeamStartPos.transform.position;
        }
        else
        {
            return blueTeamStartPos.transform.position;
        }
    }
}
