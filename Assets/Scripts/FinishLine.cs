using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float reloadDelay = 1f; // Delay before reloading the scene
    [SerializeField] private ParticleSystem finishEffect; // Particle effect to play when the player crosses the finish line

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has crossed the finish line!");
            finishEffect.Play(); // Play the finish effect particles      

            Invoke(nameof(ReloadScene), reloadDelay); // Reload the scene after the specified delay

        }
    }

    void ReloadScene()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void NextLevel()
    {
        int UnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // Get the highest unlocked level from PlayerPrefs
        PlayerPrefs.SetInt("UnlockedLevel", UnlockedLevel + 1); // Unlock the next level 
        PlayerPrefs.Save();
        if (UnlockedLevel > 6)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            SceneManager.LoadScene($"Level_{UnlockedLevel + 1}"); // Load the next level
        }
    }

}
