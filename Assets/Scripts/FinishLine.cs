using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float reloadDelay = 1.5f; // Delay before loading next level
    [SerializeField] private ParticleSystem finishEffect; // Particle effect to play when the player crosses the finish line

    private bool hasFinished = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasFinished)
        {
            hasFinished = true;
            Debug.Log("Player has crossed the finish line!");

            if (finishEffect != null)
            {
                finishEffect.Play();
            }

            // Disable player control so they coast smoothly
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.CanControlPlayer = false;
            }

            Invoke(nameof(NextLevel), reloadDelay);
        }
    }

    void NextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int currentLevelIndex = 1;

        string currentSceneName = SceneManager.GetActiveScene().name;
        // Parse current scene number, e.g. "Level1" -> 1
        if (currentSceneName.StartsWith("Level") && int.TryParse(currentSceneName.Substring(5), out int lvl))
        {
            currentLevelIndex = lvl;
        }

        // Unlock next level if we beat our highest unlocked level
        if (currentLevelIndex >= unlockedLevel)
        {
            unlockedLevel = currentLevelIndex + 1;
            PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel);
            PlayerPrefs.Save();
        }

        int nextLevelIndex = currentLevelIndex + 1;
        string nextSceneName = "Level" + nextLevelIndex;

        // If next level doesn't exist or past level 6, go to LevelSelector
        if (nextLevelIndex > 6 || !Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene("LevelSelector");
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
