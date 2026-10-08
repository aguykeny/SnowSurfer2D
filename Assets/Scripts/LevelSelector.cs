using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // Get the highest unlocked level from PlayerPrefs

    Transform levels= transform.GetChild(0); // Get the parent object that contains all level buttons  

    for (int i = 0; i < levels.childCount; i++)
        {
            Button levelbutton = levels.GetChild(i).GetComponent<Button>();
            levelbutton.interactable = i < unlockedLevel; // Enable the button if the level is unlocked, otherwise disable it
        }      
    }

}
