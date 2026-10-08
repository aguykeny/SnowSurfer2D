using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
  public void PlayGame()
  {
    // Load the main game scene
    SceneManager.LoadScene("LevelSelector");
  }
  public void OpenCredits()
  {
    // Load the credits scene
    SceneManager.LoadScene("Credits");
  }

  public void OpenCharacterSelect()
  {
    // Load the character select scene
    SceneManager.LoadScene("CharecterSelect");
  }
   public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
       public void BackToMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Menu");
    }
    public void GoToLevel(int level)
    {
        // Load the specified level scene
        SceneManager.LoadScene("Level" + level);
    }
    public void SelectCharacter(int characterIndex)
    {
        // Store the selected character index in PlayerPrefs
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.Save();
        BackToMenu();
    }
}
