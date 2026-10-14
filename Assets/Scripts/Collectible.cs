using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int scorePoints = 50;
    [SerializeField] private ParticleSystem collectEffect;
    [SerializeField] private float bobbingSpeed = 3f;
    [SerializeField] private float bobbingAmount = 0.2f;

    private Vector3 startPos;
    private bool isCollected = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Smooth floating/bobbing visual effect
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;

            ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddScore(scorePoints);
            }

            if (collectEffect != null)
            {
                collectEffect.Play();
                collectEffect.transform.SetParent(null);
                Destroy(collectEffect.gameObject, collectEffect.main.duration);
            }

            Destroy(gameObject);
        }
    }
}
