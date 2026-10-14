using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int currentScore = 0;

    public int CurrentScore => currentScore;

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreText();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString("00000");
        }
    }
}
