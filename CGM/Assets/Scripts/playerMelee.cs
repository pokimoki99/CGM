using Fusion;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class playerMelee : NetworkBehaviour
{
    [Networked] private TickTimer delay { get; set; }

    public GameObject WeaponObject;
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
                moveCorouTine = StartCoroutine(MeleeAttackArc());

            }
        }
    }

    public bool isGuarding()
    {
        if (Input.GetButton("Fire2") && weaponType == "OH_Sword") //rightclick block
        {
            return true;
        }
        else
        {
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
        startRotation = WeaponObject.transform.localRotation;
        while (elapsedTime < attackDuration)
        {
            float rotationAngle = 90f * (elapsedTime / attackDuration);
            WeaponObject.transform.localRotation = WeaponObject.transform.localRotation * Quaternion.Euler(0, 0, -rotationAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(attackDelay);

        float returnTime = 0f;
        float returnDuration = 0.5f;
        Quaternion endRotation = startRotation;
        while (elapsedTime < returnDuration)
        {
            WeaponObject.transform.localRotation = Quaternion.Slerp(WeaponObject.transform.localRotation, endRotation, returnTime / returnDuration);
            returnTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRotation;
        isMoving = false;
        moveCorouTine = null;
    }
}