using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private float crashDelay = 1f; // Delay before reloading the scene
    [SerializeField] private ParticleSystem crashEffect;
    [SerializeField] private float spawnImmunity = 1.2f; // Time in seconds player cannot crash after spawning

    private PlayerController playerController;
    private Rigidbody2D rb;
    private bool hasCrashed = false;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
        playerController = GetComponent<PlayerController>() ?? FindAnyObjectByType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (other.gameObject.layer == layerIndex && !hasCrashed)
        {
            TriggerCrash();
        }
    }

    public void TriggerCrash()
    {
        if (hasCrashed) return;
        if (Time.time - spawnTime < spawnImmunity) return; // Ignore collisions right after spawn

        // Check if player has active shield power-up
        if (playerController != null && playerController.HasShield)
        {
            playerController.ConsumeShield();
            return;
        }

        hasCrashed = true;

        if (playerController != null)
        {
            playerController.CanControlPlayer = false; // Disable player input
        }

        // Freeze player physics so they don't sink through the floor during the delay
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (crashEffect != null)
        {
            crashEffect.Play();
        }

        Invoke(nameof(ReloadScene), crashDelay);
    }

    void ReloadScene()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
