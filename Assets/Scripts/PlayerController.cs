using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement & Control")]
    [SerializeField] private float torqueAmount = 20f;
    [SerializeField] private float bootsSpeed = 35f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallDeathThreshold = -25f;

    [Header("Visual Effects & Score")]
    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject shieldVisual; // Optional shield visual indicator

    private SurfaceEffector2D surfaceEffector2D;
    private Rigidbody2D rb;

    private float baseSpeed;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;

    private bool canControlPlayer = true;
    private float totalRotation;
    private float previousRotation;
    private int flipCount;

    private bool isGrounded = false;
    private bool hasShield = false;

    public bool CanControlPlayer { get => canControlPlayer; set => canControlPlayer = value; }
    public bool HasShield => hasShield;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ensure continuous collision detection to completely prevent tunneling through thin edge colliders
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    void Start()
    {
        // Safe character model activation
        if (transform.childCount > 0 && transform.GetChild(0).childCount > 0)
        {
            Transform characterRoot = transform.GetChild(0);
            int selectedIndex = Mathf.Clamp(PlayerPrefs.GetInt("SelectedCharacter", 0), 0, characterRoot.childCount - 1);
            for (int i = 0; i < characterRoot.childCount; i++)
            {
                characterRoot.GetChild(i).gameObject.SetActive(i == selectedIndex);
            }
        }

        // Input actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        surfaceEffector2D = FindAnyObjectByType<SurfaceEffector2D>();
        if (surfaceEffector2D != null)
        {
            baseSpeed = surfaceEffector2D.speed;
            // Push center of mass directly to prevent rotational whipping on spawn
            surfaceEffector2D.useContactForce = false;
            surfaceEffector2D.useBounce = false;
            surfaceEffector2D.useFriction = true;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        previousRotation = transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        if (!canControlPlayer) return;

        // Read input during Update
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }

        HandleJump();
        BoostPlayer();
        CalculateFlips();
        CheckFallDeath();
    }

    void FixedUpdate()
    {
        if (!canControlPlayer) return;

        // Apply physical torque in FixedUpdate for framerate-independent physics
        ApplyTorque();
    }

    private void ApplyTorque()
    {
        if (moveInput.x < 0)
        {
            rb.AddTorque(torqueAmount);
        }
        else if (moveInput.x > 0)
        {
            rb.AddTorque(-torqueAmount);
        }
    }

    private void HandleJump()
    {
        bool jumpPressed = (jumpAction != null && jumpAction.WasPressedThisFrame())
                           || (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);

        if (jumpPressed && isGrounded)
        {
            // Apply upward jump impulse perpendicular to the board
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void BoostPlayer()
    {
        if (surfaceEffector2D == null) return;

        if (moveInput.y > 0)
        {
            surfaceEffector2D.speed = Mathf.Max(bootsSpeed, baseSpeed + 10f);
        }
        else
        {
            surfaceEffector2D.speed = baseSpeed;
        }
    }

    private void CalculateFlips()
    {
        float currentRotation = transform.rotation.eulerAngles.z;
        totalRotation += Mathf.DeltaAngle(previousRotation, currentRotation);

        if (Mathf.Abs(totalRotation) >= 340f)
        {
            flipCount++;
            int pointsEarned = flipCount * 100;
            if (scoreManager != null)
            {
                scoreManager.AddScore(pointsEarned);
            }
            totalRotation = 0f;
        }

        previousRotation = currentRotation;
    }

    private void CheckFallDeath()
    {
        if (transform.position.y < fallDeathThreshold)
        {
            CrashDetector crashDetector = GetComponent<CrashDetector>();
            if (crashDetector != null)
            {
                crashDetector.TriggerCrash();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            isGrounded = true;
            if (snowEffect != null && !snowEffect.isPlaying)
            {
                snowEffect.Play();
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            isGrounded = false;
            if (snowEffect != null)
            {
                snowEffect.Stop();
            }
        }
    }

    public void ApplyPowerUp(PowerUpScriptableObject powerUpData)
    {
        if (powerUpData == null) return;

        if (powerUpData.PowerUpType == "Speed")
        {
            StartCoroutine(SpeedPowerUpRoutine(powerUpData.PowerUpValue, powerUpData.TimeLimit));
        }
        else if (powerUpData.PowerUpType == "Shield")
        {
            ActivateShield();
        }
        else if (powerUpData.PowerUpType == "Rocket" || powerUpData.PowerUpType == "Launcher")
        {
            ApplyRocketBoost(powerUpData.PowerUpValue, powerUpData.TimeLimit);
        }
    }

    public void ApplyRocketBoost(float force, float duration)
    {
        if (rb != null)
        {
            // Propel diagonally forward and upward with strong rocket thrust
            Vector2 launchDir = (transform.right * 1.6f + transform.up * 1.3f).normalized;
            rb.linearVelocity = launchDir * (force > 0 ? force : 30f);
        }
        StartCoroutine(SpeedPowerUpRoutine(force > 0 ? force * 0.5f : 15f, duration > 0 ? duration : 3f));
    }

    private IEnumerator SpeedPowerUpRoutine(float speedBoost, float duration)
    {
        baseSpeed += speedBoost;
        bootsSpeed += speedBoost;

        yield return new WaitForSeconds(duration);

        baseSpeed -= speedBoost;
        bootsSpeed -= speedBoost;

        if (surfaceEffector2D != null && moveInput.y <= 0)
        {
            surfaceEffector2D.speed = baseSpeed;
        }
    }

    public void DeactivatePowerUp(PowerUpScriptableObject powerUpData)
    {
        // Kept for backward compatibility; durations are managed by Coroutines in ApplyPowerUp
    }

    public void ActivateShield()
    {
        hasShield = true;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }
        else
        {
            // Visual feedback: tint active character sprite to cyan
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.3f, 0.9f, 1f, 1f);
            }
        }
    }

    public void ConsumeShield()
    {
        hasShield = false;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
        else
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.white;
            }
        }

        // Small upward bounce upon breaking shield to save the player from getting stuck
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);
        }
    }
}
