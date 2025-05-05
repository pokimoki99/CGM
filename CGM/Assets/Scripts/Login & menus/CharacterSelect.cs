using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    public GameObject[] characters;
    public int selectedCharacter = 0;
    public TMP_Text characterText;

    private void Awake()
    {
        UpdateText();
    }

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        if (selectedCharacter > 3)
        {
            selectedCharacter = 0;
        }
        characters[selectedCharacter].SetActive(true);
        UpdateText();
        Debug.Log(selectedCharacter);
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);
        UpdateText();
        Debug.Log(selectedCharacter);
    }

    public void UpdateText()
    {
        if (selectedCharacter == 0)
        {
            characterText.text = "character type 1";
        }
        else if (selectedCharacter == 1)
        {
            characterText.text = "character type 2";
        }
        else if (selectedCharacter == 2)
        {
            characterText.text = "character type 3";
        }
    }

    public void LoadGame()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}