using Fusion;
using UnityEngine;

public class ProjectileCode : NetworkBehaviour
{
    public NetworkTransform networkTransform;
    private Vector3 velocity;
    public float lifeTime = 2f;
    private TickTimer despawnTimer;
    public int damage = 50;

    public void Initialize (Vector3 initVelocity)
    {
        velocity = initVelocity;
        despawnTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            transform.position += velocity * Runner.DeltaTime;

            if (despawnTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyScript>().enemyDamaged(damage);
            Destroy(gameObject);
        }
    }
}
