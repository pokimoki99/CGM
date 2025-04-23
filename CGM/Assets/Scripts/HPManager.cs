using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    public static HPManager instance { get; private set; }

    public GameObject HP_barPrefab;
    public Transform canvasTransform;
    public float initYPos = 100f;
    public float initXPos = -100f;
    public float spacing = 20f;

    private Dictionary<NetworkObject, GameObject> playerHealthbars = new Dictionary<NetworkObject, GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void registerPlayer(NetworkObject playerOBJ)
    {
        if (!playerHealthbars.ContainsKey(playerOBJ))
        {
            var hpBar = Instantiate(HP_barPrefab, canvasTransform);
            hpBar.transform.localPosition = new Vector3(initXPos, initYPos - spacing * playerHealthbars.Count, 0);

            Health playerHP = playerOBJ.GetComponent<Health>();
            if (playerHP != null)
            {
                playerHP.healthBarFill =  hpBar.GetComponentInChildren<Image>();
                playerHP.UpdateUI();
            }
            playerHealthbars.Add(playerOBJ,hpBar);
        }
    }
    public void removePlayer(NetworkObject playerOBJ)
    {
        if (playerHealthbars.TryGetValue(playerOBJ, out var hpBar))
        {
            Destroy(hpBar);
            playerHealthbars.Remove(playerOBJ);
        }
        RearrangeHPBar();
    }
    private void RearrangeHPBar()
    {
        int index = 0;
        foreach (var player in playerHealthbars)
        {
            player.Value.transform.localPosition = new Vector3(initXPos, initYPos - spacing * index++, 0);
        }
    }
}
