using Fusion;
using UnityEngine;

public class ProjectileCode : NetworkBehaviour
{
    public NetworkTransform networkTransform;
    private Vector3 velocity;
    //public float lifeTime = 2f;
    //private TickTimer despawnTimer;
    public int damage = 50;

    public float maxDistance = 200f;
    public Vector3 axisM = new Vector3(1, 0, 1);

    [Networked] private Vector3 startPos { get; set;}
    [Networked] private float SumDistance { get; set; }

    public void Initialize (Vector3 initVelocity)
    {
        velocity = initVelocity;
        startPos = transform.position;
        SumDistance = 0f;
        //despawnTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            Vector3 previousPos = transform.position;
            transform.position += velocity * Runner.DeltaTime;

            Vector3 movement  = Vector3.Scale(transform.position - previousPos, axisM);

            SumDistance += movement.magnitude;
            if (SumDistance > maxDistance)
            {
                Runner.Despawn(Object);
            }
            /*if (despawnTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }*/
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyScript>().enemyDamaged(damage);
            Runner.Despawn(Object);
        }
        if (other.CompareTag("Boss"))
        {
            other.GetComponent<BossScript>().bossDamaged(damage);
            Runner.Despawn(Object);
        }
    }
}
