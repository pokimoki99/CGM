using Fusion;
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

    bool phaseOne, phaseTwo, phaseThree = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        isVulnerable = true;
        enemySpawn = gameObject.GetComponent<enemySpawner>();
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
        if (HasStateAuthority && tickTimer.Expired(Runner))
        {
            checkPhase();
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

}
