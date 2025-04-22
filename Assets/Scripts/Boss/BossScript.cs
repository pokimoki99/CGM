using Fusion;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : NetworkBehaviour
{
    [Networked] public float currentHP { get; set; }
    public float maxHP;

    float[] healthPhases = {0.75f, 0.5f, 0.25f};
    private enemySpawner enemySpawn;
    float waveTimer = 10f;
    float invulnerability = 3f;

    [Networked] private TickTimer tickTimer { get; set; }
    [Networked] private int currThreshold { get; set; }
    [Networked] private NetworkBool isVulnerable { get; set; }

    // Difficulty 2 and 3
    public GameObject IsolatedPunishment, HealthCleave;

    [Networked ]private TickTimer punishmentCD { get; set; }
    [Networked ]private TickTimer cleaveCD { get; set; }
    [Networked ]private TickTimer channelTimer { get; set; }
    [Networked] private NetworkBool isChanneling { get; set; }
    private float attackInterval = 10f;
    private float channelDuration = 1f;
    private int difficulty = 0;
    private float safeDistance = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        isVulnerable = true;
        enemySpawn = gameObject.GetComponent<enemySpawner>();

        if (HasStateAuthority)
        {
            punishmentCD = TickTimer.CreateFromSeconds(Runner, attackInterval);
            IsolatedPunishment.SetActive(false);
            HealthCleave.SetActive(false);
        }    
    }

    public void bossDamaged(float damage)
    {
        if (HasStateAuthority && isVulnerable)
        {
            currentHP = Mathf.Max(currentHP, damage, 0);
            phaseTrigger();
        }
    }
    void phaseTrigger()
    {
        if (currThreshold >= healthPhases.Length) return;

        float threshold = maxHP * healthPhases[currThreshold];
        if (currentHP <= threshold)
        {
            startWave();
            currThreshold++;
        }
    }

    void startWave()
    {
        isVulnerable = false;
        tickTimer = TickTimer.CreateFromSeconds(Runner, waveTimer);
        enemySpawn.summonElite();
    }

    private void FixedUpdate()
    {
        if (HasStateAuthority)
        {
            if (tickTimer.Expired(Runner))
            {
                checkPhase();
            }
            channelAttack();
        }
    }

    void checkPhase()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            tickTimer = TickTimer.CreateFromSeconds(Runner, invulnerability);
            isVulnerable = false;
        }
        else
        {
            tickTimer = TickTimer.CreateFromSeconds(Runner, 1f); //gives the runner more time //need to change to delete current enemies and spawn new ones with less hp
        }
    }

    void channelAttack()
    {
        if (punishmentCD.ExpiredOrNotRunning(Runner) && !isChanneling)
        {
            startChanneling();
        }
        if (isChanneling && channelTimer.Expired(Runner))
        {
            triggerAttack();
        }
    }

    void startChanneling()
    {
        isChanneling = true;
        if (difficulty == 2)
        {
            channelDuration = 10;
            punishmentCD = TickTimer.CreateFromSeconds(Runner, attackInterval);
        }
        else if (difficulty == 3)
        {
            channelDuration = 10;
            cleaveCD = TickTimer.CreateFromSeconds(Runner, attackInterval);
        }
        channelTimer = TickTimer.CreateFromSeconds(Runner, channelDuration);
        Rpc_playChannelEffect();
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    void Rpc_playChannelEffect()
    {
        if (difficulty == 2)
        {
            IsolatedPunishment.SetActive(true);
        }
        else if (difficulty == 3)
        {
            HealthCleave.SetActive(true);
        }
    }

    void triggerAttack()
    {
        isChanneling = false;
        
        Rpc_playAttackEffects();
    }

    void checkPlayerSeparation()
    {
        List<PlayerRef> players = new List<PlayerRef>();
        foreach (var player in Runner.ActivePlayers)
        {
            if (Runner.TryGetPlayerObject(player, out var playerOBJ))
            {
                players.Add(player);
            }
        }
        foreach (var player in players)
        {
            if (isPlayerIsolated(player, players))
            {
                damageIsolatedPlayer(player);
            }
        }
    }
    bool isPlayerIsolated(PlayerRef player, List<PlayerRef> players)
    {
        if (players.Count <= 1) return false;
        Vector3 targetPos = getPlayerPosition(player);

        foreach (var otherPlayer in players)
        {
            if (otherPlayer == player) continue;

            Vector3 otherPos = getPlayerPosition(otherPlayer);
            if (Vector3.Distance(targetPos, otherPos) <= safeDistance)
            {
                return false;
            }
        }
        return true;
    }
    Vector3 getPlayerPosition(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var playerOBJ))
        {
            return playerOBJ.transform.position;
        }
        return Vector3.zero;
    }

    void damageIsolatedPlayer(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var playerOBJ) && playerOBJ.TryGetComponent<Health>(out var health))
        {
            health.dealDamageRPC(30);
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void Rpc_playAttackEffects()
    {
        if (difficulty == 2)
        {
            if (HasStateAuthority)
            {
                checkPlayerSeparation();
                IsolatedPunishment.SetActive(true);
            }
        }
        else if (difficulty == 3)
        {
            HealthCleave.SetActive(false);
        }
    }
}
