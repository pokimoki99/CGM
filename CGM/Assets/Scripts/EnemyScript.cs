using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : NetworkBehaviour
{
    NavMeshAgent agent;
    GameObject[] players;
    GameObject closestPlayer;
    public float agentSpeed = 5;
    public RNGSystem rng;
    public bool poison = false;
    public float playerExpertise;
    int poisonCount;
    int bleedCount;
    int HemorrhageCount;
    [Networked] private TickTimer poisonTimer { get; set; }
    [Networked] private TickTimer bleedTimer { get; set; }
    [Networked] private TickTimer HemorrhageTimer { get; set; }
    [Networked] private TickTimer StunTimer { get; set; }
    [Networked] private TickTimer attackTimer { get; set; }

    bool isHemorrhaging = false;

    [Networked] public int xpValue { get; set; } = 10;
    [Networked] public int health { get; set; } = 100;
    [Networked] public int damage { get; set; } = 10;
    [Networked] public bool isAttacking { get; set; } = false;
    [Networked] public bool isStunned { get; set; } = false;
    public static event System.Action<int> onEnemyKilled;

    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        players = GameObject.FindGameObjectsWithTag("Player");
        closestPlayer = FindNearestPlayer();
        agent.SetDestination(closestPlayer.transform.position);
        agent.speed = agentSpeed;
    }

    GameObject FindNearestPlayer()
    {
        float distance = Mathf.Infinity;
        foreach (var p in players)
        {
            float posDiff = (p.transform.position - agent.transform.position).sqrMagnitude;
            if (posDiff < distance)
            {
                closestPlayer = p;
                distance = posDiff;
            }
        }
       return closestPlayer;
    }
    private void FixedUpdate()
    {
        closestPlayer = FindNearestPlayer();

        if (closestPlayer != null && attackTimer.ExpiredOrNotRunning(Runner))
        {
            float dist = Vector3.Distance(agent.transform.position, closestPlayer.transform.position);
            if (dist < 5f)
            {
                agent.isStopped = true;
                attackTimer = TickTimer.CreateFromSeconds(Runner, 1f);
                animator.SetFloat("MoveSpeed", 0);
                animator.SetTrigger("Attack");
            }
            else
            {
                agent.isStopped = false;
                animator.SetFloat("MoveSpeed", 1);
                agent.SetDestination(closestPlayer.transform.position);
            }
        }

        if (isHemorrhaging && HemorrhageTimer.Expired(Runner))
        {
            isHemorrhaging = false;
            HemorrhageCount = 0;
        }
        if (poisonTimer.Expired(Runner))
        {
            enemyDamaged((int)(poisonCount + (poisonCount * (0.2f + (playerExpertise / 100)))));
        }
        if (isStunned && !StunTimer.Expired(Runner))
        {
            agent.speed = 0;
            StunTimer = TickTimer.CreateFromSeconds(Runner, 1f);
        }
    }
    public void enemyDamaged(int damage)
    {
        if (Object.HasStateAuthority)
        {
            if (isHemorrhaging)
            {
                damage = (int)(damage + (damage * (0.3f + (HemorrhageCount / 100))));
            }    
            health -= damage;
            if (health <= 0)
            {
                Die();
                animator.SetTrigger("Dead");
            }
        }  
    }
    public void dotEffect(string dotType, float Expertise)
    {
        playerExpertise = Expertise;
        if (dotType == "Poison")
        {
            poisonCount++;
            poisonTimer = TickTimer.CreateFromSeconds(Runner, 5f);
        }
        if (dotType == "Bleed")
        {
            bleedCount++;
            bleedTimer = TickTimer.CreateFromSeconds(Runner, 10f);
            if (bleedCount == 10)
            {
                bleedCount=0;
                HemorrhageCount++;
                if (!isHemorrhaging)
                {
                    HemorrhageTimer = TickTimer.CreateFromSeconds(Runner, 15f);
                }
            }   
        }    
    }
    private void Die()
    {
        if (Object.HasStateAuthority)
        {
            onEnemyKilled?.Invoke(xpValue);
            rng.ItemDrop();
            Runner.Despawn(Object);
            Debug.Log("gained xp");
        }
    }
    public void noDrop()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
