using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] private PowerUpScriptableObject powerUpData;
    [SerializeField] private ParticleSystem pickupEffect;

    private bool isCollected = false;

    public PowerUpScriptableObject PowerUpData { get => powerUpData; set => powerUpData = value; }

    public void Init(PowerUpScriptableObject data)
    {
        powerUpData = data;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            PlayerController playerController = other.GetComponent<PlayerController>() ?? FindAnyObjectByType<PlayerController>();

            if (playerController != null && powerUpData != null)
            {
                playerController.ApplyPowerUp(powerUpData);
            }

            if (pickupEffect != null)
            {
                pickupEffect.Play();
                pickupEffect.transform.SetParent(null);
                Destroy(pickupEffect.gameObject, pickupEffect.main.duration);
            }

            Destroy(gameObject);
        }
    }
}
