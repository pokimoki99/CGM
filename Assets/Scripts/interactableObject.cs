using Fusion;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class interactableObject : NetworkBehaviour
{
    public string interaction;
    public string weaponType = "none";
    public float damage, fireRate;
    public int ammo;
    public GameObject interactionOther;
    float elapsedTime = 0f;

    private Vector3 startPos, endPos, pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pos = interactionOther.transform.position;
        endPos = new Vector3(pos.x,pos.y - 10,pos.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interaction()
    {
        StartCoroutine(movingObject());
    }
    IEnumerator movingObject()
    {
        while (elapsedTime < 2f)
        {
            elapsedTime += Time.deltaTime;
            interactionOther.transform.position = Vector3.Lerp(pos, endPos, elapsedTime);
            yield return null;
        }
        Destroy(interactionOther.gameObject);
        Destroy(this.gameObject);
    }
    public void publicDestroy()
    {
        Destroy(this.gameObject);
    }

}
