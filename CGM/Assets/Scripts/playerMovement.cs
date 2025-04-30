using Fusion;
using System.Collections;
//using UnityEditor.Timeline;
using UnityEngine;


public class playerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;

    private CharacterController _controller;

    public float PlayerSpeed;
    public float Speed;

    public float JumpForce = 5f;
    public float GravityValue = -9.81f;
    public Camera Camera;
    public playerShooter playerAttack;

    public bool twoDee, isDowned = false;

    public float cameraRotx, cameraRoty, cameraRotz, cameraRotBob, cameraPosx, cameraPosY, cameraPosZ = 0f; //testing values

    Health health;
    playerMelee melee;
    public string characterType;

    public Animator animator;
    public override void Spawned()
    {
        
        Debug.Log($"Position at start of Spawned: {transform.position}");

        if (_controller != null)
        {
            _controller.enabled = false;
            StartCoroutine(EnableControllerAfterDelay());
        }
        health = gameObject.GetComponent<Health>();
        melee = gameObject.GetComponent<playerMelee>();
    }
    private IEnumerator EnableControllerAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        _controller.enabled = true;
        Debug.Log($"Position after re-enabling controller: {transform.position}");
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>(); 
        playerAttack = GetComponent<playerShooter>();
        if (HasStateAuthority)
        {
            Camera = Camera.main;
            if (!twoDee)
            {
                Camera.GetComponent<FirstPersonCamera>().Target = transform;
            }

        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Application.Quit();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // FixedUpdateNetwork is only executed on the StateAuthority
        if (!twoDee)
        {
            if (_controller.isGrounded)
            {
                _velocity = new Vector3(0, -1, 0);
            }
            Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
            Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;

            _velocity.y += GravityValue * Runner.DeltaTime;
            if (_jumpPressed && _controller.isGrounded)
            {
                _velocity.y += JumpForce;
            }
            _controller.Move(move + _velocity * Runner.DeltaTime);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }

            _jumpPressed = false;
        }
        else
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;

            _controller.enabled = true;
            _controller.Move(move);
            Camera.transform.position = new Vector3(transform.position.x +cameraPosx, 13 + cameraPosY, transform.position.z - 4 + cameraPosZ);
            //Camera.transform.rotation = new Quaternion(0, 0, 0, 0);
            Camera.transform.rotation = new Quaternion(0.8f + cameraRotx, +cameraRoty, +cameraRotz, 1 + cameraRotBob);


            //camera adjustment for animation
            Vector3 worldMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (worldMove.sqrMagnitude > 0.01f)
            {
                Vector3 facingFor = transform.forward;

                Vector3 localMove = transform.InverseTransformDirection(worldMove);
                animator.SetFloat("Vert", localMove.z);
                animator.SetFloat("Horz", localMove.x);
            }
            else
            {
                animator.SetFloat("Vert", 0);
                animator.SetFloat("Horz", 0);
            }

        }
        if (health.Downed())
        {
            PlayerSpeed = 0.1f;
            isDowned = true;
            animator.SetBool("isDowned", true);
        }
        else
        {
            animator.SetBool("isDowned", false);
            if (melee.weaponType == "GreatSword" || melee.weaponType == "Mace")
            {
                PlayerSpeed = Speed - (Speed * melee.speedReduction);
            }
            else
            {
                if (characterType == "Fighter")
                {
                    PlayerSpeed = Speed + melee.speed;
                }
                else
                {
                    PlayerSpeed = Speed;
                }
            }
        }

    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "interactable")
        {
            string interaction = other.GetComponent<interactableObject>().interaction;
            interactableObject interact = other.GetComponent<interactableObject>();
            //other.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            if (interaction == "wall")
            {
                interact.Interaction();
            }
            else if (interaction == "speed")
            {
                interact.publicDestroy();
                StartCoroutine(speedBoost(Random.Range(2f, 5f)));
            }
            else if (interaction == "fire_rate")
            {
                interact.publicDestroy();
                StartCoroutine(fireRateBoost(Random.Range(2f, 5f)));
            }
            else if (interaction == "WeaponChange")
            {
                playerAttack.WeaponChange(interact.weaponType, interact.damage, interact.fireRate, interact.ammo);
                interact.publicDestroy();
                animator.SetTrigger("pickUp");
            }
            else
            {
                other.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            }
        }
        
        if (other.tag == "Spawner")
        {
            other.GetComponent<enemySpawner>().enemySpawnerActivator = true;
        }  
        if (other.tag == "Enemy")
        {
            int damage = other.GetComponent<EnemyScript>().damage;
            if (melee.isGuarding())
            {
                other.GetComponent<EnemyScript>().enemyDamaged(damage*2);
            }
            else
            {
                if (other.GetComponent<EnemyScript>().isAttacking && !melee.isAttacking)//enemyAttack
                {
                    health.dealDamageRPC(damage - (damage * melee.damageReduction));
                    other.GetComponent<EnemyScript>().noDrop();
                    animator.SetTrigger("Hit");

                    if (health.Downed())
                    {
                        PlayerSpeed = 0.1f;
                        isDowned = true;
                    }
                    else
                    {
                        PlayerSpeed = 10f;
                    }
                }
                else//playerAttack
                {
                    other.GetComponent<EnemyScript>().enemyDamaged((int)(damage + melee.damage + melee.strength + (melee.agility*2)));
                }
            }
        }
        if (other.tag == "Player" && HasStateAuthority)
        {
            Debug.Log("V is not pressed");
            if (other.GetComponentInParent<playerMovement>().isDowned && Input.GetKey(KeyCode.V))
            {
                other.GetComponentInParent<playerMovement>().revived();
                other.GetComponentInParent<playerMovement>().PlayerSpeed = 10f;
                Debug.Log("player is downed and trying to get healed");

            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && HasStateAuthority)
        {
            if (other.GetComponentInParent<playerMovement>().isDowned && Input.GetKey(KeyCode.V))
            {
                other.GetComponentInParent<playerMovement>().revived();
                other.GetComponentInParent<playerMovement>().PlayerSpeed = 10f;
            }
        }
    }

    public void revived()
    {
        isDowned = false;
        PlayerSpeed = 10f;
        Health health = gameObject.GetComponent<Health>();
        health.dealDamageRPC(-100);
        Debug.Log("player is downed and trying to get healed");
    }

    private IEnumerator speedBoost(float duration)
    {
        Speed += 20;
        yield return new WaitForSeconds(duration);
        Speed -= 20;
    }
    private IEnumerator fireRateBoost(float duration)
    {
        playerAttack.cooldownTime = 0.01f;
        yield return new WaitForSeconds(duration);
        playerAttack.cooldownTime = 0.5f;
    }

}