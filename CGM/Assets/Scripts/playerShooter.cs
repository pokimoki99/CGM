using Fusion;
using Mono.Cecil;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class playerShooter : NetworkBehaviour
{
    [Networked] private TickTimer delay {  get; set; }

    public GameObject projectilePrefab;

    public GameObject WeaponObject;
    public String characterType = "Shooter";
    public String weaponType = "Pistol";

    public int attacks = 0;
    //shooter
    public float projectileDamage, projectileSpeed = 10f;
    public float cooldownTime = 0.5f;
    public int ammo;
    public float projectileLifespan;
    // Update is called once per frame

    playerAttack playerStats;
    playerMovement playerMove;

    public int magazine_size, reload_speed, expertise;

    private void Start()
    {
        characterTransform(characterType);
        playerStats = gameObject.GetComponent<playerAttack>();
        playerMove = gameObject.GetComponent<playerMovement>();
    }

    public override void FixedUpdateNetwork()
    {
        if (characterType == "Shooter")
        {
            if (Input.GetButton("Fire1") && delay.ExpiredOrNotRunning(Runner))
            {
                characterTransform(characterType);
                delay = TickTimer.CreateFromSeconds(Runner, cooldownTime);
                Shoot();
            }
        }

    }

    void Shoot()
    {
        if (HasStateAuthority)
        {
            Vector3 spawnPos = transform.position + transform.forward;
            if (weaponType == "Pistol")
            {
                cooldownTime = 0.5f;
                ProjSpawn(spawnPos);
            }
            if (weaponType == "Shotgun")
            {
                cooldownTime = 0.5f;
                ProjSpawn(spawnPos);
                for (int i = 0; i < 2; i++)
                {
                    if (i % 2 == 0)
                    {
                        spawnPos = transform.position + transform.right;
                        ProjSpawn(spawnPos);
                    }
                    else
                    {
                        spawnPos = transform.position - transform.right;
                        ProjSpawn(spawnPos);
                    }
                }
            }
            if (weaponType == "Rifle")
            {
                cooldownTime = 0.0001f;
                ProjSpawn(spawnPos);
            }
        }
    }
    void ProjSpawn(Vector3 spawnPos)
    {
        NetworkObject projectileObj = Runner.Spawn(projectilePrefab, spawnPos, transform.rotation, Object.InputAuthority);
        if (projectileObj != null)
        {
            projectileObj.GetComponent<ProjectileCode>().Initialize(transform.forward * projectileSpeed);
            projectileObj.GetComponent<ProjectileCode>().damage = (int)(projectileDamage + playerStats.playerStrength);
            projectileObj.GetComponent<ProjectileCode>().maxDistance = projectileLifespan;
            if (gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude> 0)
            {
                projectileObj.GetComponent<ProjectileCode>().damage = (int)(projectileDamage + playerStats.playerStrength)*2;
            }
        }
    }
    void characterTransform(string charType)
    {
        if (charType == "Shooter")
        {
            WeaponObject.transform.localPosition = new Vector3(0, 0, 1);
        }
    }
    public void WeaponChange(string weaponType, float damage, float fireRate, int ammo)
    {
        this.weaponType = weaponType;
        projectileDamage = damage;
        cooldownTime = fireRate;
        this.ammo = ammo;
    }
}