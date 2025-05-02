using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
   public static UIManager Singleton
    {
        get => _singleton;
        set
        {
            if (value == null)
                _singleton = null;
            else if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Destroy(value);
                Debug.LogError($"There should only ever be one instance of {nameof(UIManager)}!");
            }
        }

    }

    private static UIManager _singleton;

    [SerializeField] private Slider healthSlider;

    [SerializeField] private Slider xp_Slider;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private TextMeshProUGUI weaponText;

    public Health localHealth;
    public XP_System localXP;
     
    private void Awake()
    {
        Singleton = this;

        healthSlider.value = localHealth.networkedHealth;
        xp_Slider.value = 0f;
        weaponText.text = "None";
    }

    private void Update()
    {
        if (localHealth == null)
            return;
        if (localXP == null)
            return;

        healthSlider.value = localHealth.networkedHealth;
        xp_Slider.value = localXP.currentXP;


        levelText.text = $"Level: {localXP.currentLevel}";
    }

    private void OnDestroy()
    {
        if (Singleton == this)
            Singleton = null;
    }
}
