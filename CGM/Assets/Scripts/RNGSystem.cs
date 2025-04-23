using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class RNGSystem : NetworkBehaviour
{
    public string rarity, weaponType;
    public GameObject[] weapons;
    float damage, fireRate, lifespan;
    int ammo, rarityMultiplier, damageMultiplier;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void ItemDrop()
    {
        rarity = itemRarity();
        weaponType = itemType();
        foreach (var weapon in weapons)
        {
            if (weapon.GetComponent<interactableObject>().weaponType == weaponType)
            {
                var weaponSpawn = Runner.Spawn(weapon, gameObject.transform.position, Quaternion.identity);
                weaponSpawn.GetComponent<interactableObject>().damage *= rarityMultiplier;
                weaponSpawn.GetComponent<interactableObject>().fireRate *= rarityMultiplier;
                weaponSpawn.GetComponent<interactableObject>().ammo *= rarityMultiplier;
            }
        }
    }
    string itemRarity()
    {
        int randomNum = Random.Range(0, 100000);
        if (randomNum >=0 &&  randomNum <= 100) //legendary
        {
            rarity = "Legendary";
            rarityMultiplier = 8;
        }
        else if (randomNum >= 101 &&  randomNum <= 5000) //Epic
        {
            rarity = "Epic";
            rarityMultiplier = 5;
            damageMultiplier = 4;
        }
        else if (randomNum >= 5001 &&  randomNum <= 15000) //Rare
        {
            rarity = "Rare";
            rarityMultiplier = 3;
            damageMultiplier = 2;
        }
        else if (randomNum >= 15001 &&  randomNum <= 40000) //Uncommon
        {
            rarity = "Uncommon";
            rarityMultiplier = 2;
        }
        else //between 40 000 and 100 000 // common
        {
            rarity = "Common";
            rarityMultiplier = 1;
            damageMultiplier = 0;
        }
        Debug.Log(rarity);
        return rarity;
    }
    string itemType()
    {
        int randomNum = Random.Range(0, 3);
        if (randomNum == 0) //Rifle
        {
            weaponType = "Rifle";
            damage = Random.Range(19, 22);
            lifespan = 5;
        }
        else if (randomNum == 1) //Shotgun
        {
            weaponType = "Shotgun";
        }
        else //if rand is 2 // Pistol
        {
            weaponType = "Pistol";
        }
        return weaponType;
    }
    
}
