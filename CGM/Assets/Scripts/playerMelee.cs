using Fusion;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class playerMelee : NetworkBehaviour
{
    [Networked] private TickTimer delay { get; set; }

    public GameObject WeaponObject, TwoHandedSword, OneHandedSword, Mace, Dagger;
    public String characterType = "Melee";
    public String weaponType = "Pistol";
    public Transform arcEndPoint;
    public int attacks = 0;
    //melee
    public bool isMoving;
    public float attackRange;
    public float attackDuration = 1f;
    public float attackDelay = 0.5f;
    Vector3 ObjectOrigin;
    Quaternion ObjectRot;
    Quaternion startRotation;
    Coroutine moveCorouTine;
    // Update is called once per frame

    public bool isAttacking;
    public float damage;
    public float damageReduction;
    public float speedReduction;

    //attributes
    public int strength, vitality, agility, expertise, speed;

    public Animator animator;

    private void Start()
    {
        characterTransform(characterType);
        ObjectOrigin = WeaponObject.transform.position;
        ObjectRot = WeaponObject.transform.rotation;

    }

    public override void FixedUpdateNetwork()
    {

        if (characterType == "Melee")
        {
            if (Input.GetButton("Fire1") && !isMoving)
            {
                if (moveCorouTine != null)
                {
                    StopCoroutine(moveCorouTine);
                }
                characterTransform(characterType);
                if (weaponType == "GreatSword")
                {
                    weaponChange();
                    damage += strength*4;
                    damage = (damage + (damage * 0.2f));
                    damageReduction = 0.2f + (0.02f * expertise);
                    if (damageReduction > 0.6f)
                    {
                        damageReduction = 0.6f;
                    }
                    animator.SetBool("isAttacking2H", true);
                }
                if (weaponType == "Mace")
                {
                    weaponChange();
                    damage += strength * 4;
                    damage = (damage - (damage * 0.15f));
                    animator.SetBool("isAttacking2H", true);
                }
                if (weaponType == "OH_Sword")
                {
                    weaponChange();
                    animator.SetBool("isAttacking", true);
                }
                moveCorouTine = StartCoroutine(MeleeAttackArc());

            }
        }
    }

    void weaponChange()
    {
        if (weaponType == "GreatSword")
        {
            TwoHandedSword.gameObject.SetActive(true);
            OneHandedSword.gameObject.SetActive(false);
            Mace.gameObject.SetActive(false);
            Dagger.gameObject.SetActive(false);
        }
        if (weaponType == "Mace")
        {
            TwoHandedSword.gameObject.SetActive(false);
            OneHandedSword.gameObject.SetActive(false);
            Mace.gameObject.SetActive(true);
            Dagger.gameObject.SetActive(false);
        }
        if (weaponType == "OH_Sword")
        {
            TwoHandedSword.gameObject.SetActive(false);
            OneHandedSword.gameObject.SetActive(true);
            Mace.gameObject.SetActive(false);
            Dagger.gameObject.SetActive(false);
        }
        if (weaponType == "Dagger")
        {
            TwoHandedSword.gameObject.SetActive(false);
            OneHandedSword.gameObject.SetActive(false);
            Mace.gameObject.SetActive(false);
            Dagger.gameObject.SetActive(true);
        }
    }

    public bool isGuarding()
    {
        if (Input.GetButton("Fire2") && weaponType == "OH_Sword") //rightclick block
        {
            animator.SetBool("isDefending", true);
            return true;
        }
        else
        {
            animator.SetBool("isDefending", true);
            return false;
        }
    }

    void characterTransform(string charType)
    {
        if (charType == "Melee")
        {
            WeaponObject.transform.localPosition = new Vector3(0, 0, 1.5f);
        }
    }

    IEnumerator MeleeAttackArc()
    {
        isMoving = true;
        float elapsedTime = 0f;
        //startRotation = WeaponObject.transform.localRotation;
        while (elapsedTime < attackDuration)
        {
            //float rotationAngle = 90f * (elapsedTime / attackDuration);
            //WeaponObject.transform.localRotation = WeaponObject.transform.localRotation * Quaternion.Euler(0, 0, -rotationAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(attackDelay);

        float returnTime = 0f;
        float returnDuration = 0.5f;
        //Quaternion endRotation = startRotation;
        while (elapsedTime < returnDuration)
        {
            //WeaponObject.transform.localRotation = Quaternion.Slerp(WeaponObject.transform.localRotation, endRotation, returnTime / returnDuration);
            returnTime += Time.deltaTime;
            yield return null;
        }

        //transform.localRotation = endRotation;
        isMoving = false;
        moveCorouTine = null;
        animator.SetBool("isAttacking2H", false);
        animator.SetBool("isAttacking", false);
    }
}