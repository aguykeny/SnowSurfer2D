using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float torqueAmount = 1f;
    [SerializeField] private float bootsSpeed = 35f;
    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private ScoreManager scoreManager;

    SurfaceEffector2D surfaceEffector2D;

    float baseSpeed;

    InputAction moveAction;
    Vector2 moveInput;
    Rigidbody2D rb;

    private bool canControlPlayer = true; // Flag to control player input
    private float totalRotation;
    private float previousRotation;
    private int flipCount;

    int activatePowerUpsCount;

    public bool CanControlPlayer { get => canControlPlayer; set => canControlPlayer = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Activate the selected character model
        transform.GetChild(0).GetChild(PlayerPrefs.GetInt("SelectedCharacter", 0)).gameObject.SetActive(true); 
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
        surfaceEffector2D = FindAnyObjectByType<SurfaceEffector2D>();
        baseSpeed = surfaceEffector2D.speed; // Store the initial speed of the SurfaceEffector2D
    }


    // Update is called once per frame
    void Update()
    {
        if (!canControlPlayer) return; // If player input is disabled, exit the method

        PlayerTorque();
        BoostPlayer();
        CalculateFlips();

    }

    private void CalculateFlips()
    {
        float currentRotation = transform.rotation.eulerAngles.z;
        totalRotation += Mathf.DeltaAngle(previousRotation, currentRotation);

        if (Mathf.Abs(totalRotation) >= 340f)
        {
            flipCount++;
            scoreManager.AddScore(flipCount * 100); // Update the score in ScoreManager
            totalRotation = 0; // Reset total rotation after a full flip
        }

        previousRotation = currentRotation;
    }

    /// <summary>
    /// Applies torque to the player based on the horizontal input from the Move action.
    /// </summary>
    void PlayerTorque()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput.x < 0)
        {
            rb.AddTorque(torqueAmount);
        }
        else if (moveInput.x > 0)
        {
            rb.AddTorque(-torqueAmount);
        }

    }

    void BoostPlayer()
    {
        //Increase the player's speed when the up arrow key is pressed
        //Surface Efector speed is increased to bootsSpeed
        if (moveInput.y > 0)
        {
            surfaceEffector2D.speed = bootsSpeed;
        }
        else
        {
            surfaceEffector2D.speed = baseSpeed; // Return to normal speed
        }
    }

    // Detects when the player collides with the floor and plays the snow effect
    void OnCollisionEnter2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            snowEffect.Play();
        }
    }

    // Detects when the player exits the collision with the floor and stops the snow effect
    void OnCollisionExit2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            snowEffect.Stop();
        }
    }

    internal void ApplyPowerUp(PowerUpScriptableObject powerUpData)
    {
        activatePowerUpsCount++;
        if (powerUpData.PowerUpType == "Speed")
        {
            baseSpeed += powerUpData.PowerUpValue;
            bootsSpeed += powerUpData.PowerUpValue;
        }
    }

    public void DeactivatePowerUp(PowerUpScriptableObject powerUpData)
    {
        activatePowerUpsCount--;
       if (activatePowerUpsCount == 0)
        {
            if (powerUpData.PowerUpType == "Speed")
            {
                baseSpeed -= powerUpData.PowerUpValue;
                bootsSpeed -= powerUpData.PowerUpValue;
            }
        }
    }

}
