using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : NetworkBehaviour
{
    NavMeshAgent agent;
    GameObject[] players;
    GameObject closestPlayer;
    public float agentSpeed = 10;
    public RNGSystem rng;

    [Networked] public int xpValue { get; set; } = 10;
    [Networked] public int health { get; set; } = 100;
    public static event System.Action<int> onEnemyKilled;

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
        agent.SetDestination(closestPlayer.transform.position);
    }
    public void enemyDamaged(int damage)
    {
        if (Object.HasStateAuthority)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
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
