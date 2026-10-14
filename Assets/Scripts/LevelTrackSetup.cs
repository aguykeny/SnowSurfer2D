using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class LevelTrackSetup : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private int levelOverride = 0; // 0 = auto-detect from scene name (Level1 -> 1, Level2 -> 2, etc.)
    [SerializeField] private bool autoBuildOnStart = false;

    [Header("Power-Up Data Assets")]
    [SerializeField] private PowerUpScriptableObject speedShortData;
    [SerializeField] private PowerUpScriptableObject speedLongData;
    [SerializeField] private PowerUpScriptableObject shieldData;

    [Header("Prefabs")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private GameObject powerUpPrefab;

    void Awake()
    {
        if (Application.isPlaying)
        {
            SetupLevelRuntime();
        }
        else if (autoBuildOnStart)
        {
            BuildLevel();
        }
    }

    private void SetupLevelRuntime()
    {
        int level = GetTargetLevel();

        // 1. Configure surface effector physics for current level
        SurfaceEffector2D surfaceEffector = GetComponent<SurfaceEffector2D>();
        if (surfaceEffector != null)
        {
            switch (level)
            {
                case 2:
                    surfaceEffector.speed = 27f;
                    break;
                case 3:
                    surfaceEffector.speed = 32f;
                    break;
                default:
                    surfaceEffector.speed = 22f;
                    break;
            }
            surfaceEffector.useContactForce = false;
            surfaceEffector.useBounce = false;
            surfaceEffector.useFriction = true;
        }

        // 2. Spawn dynamic items if not already saved in scene
        SpawnCollectiblesAndPowerUps(level);
    }

    private int GetTargetLevel()
    {
        int level = levelOverride;
        if (level <= 0)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.StartsWith("Level") && int.TryParse(sceneName.Substring(5), out int parsed))
            {
                level = parsed;
            }
            else
            {
                level = 1;
            }
        }
        return level;
    }

    [ContextMenu("Rebuild Level Track")]
    public void BuildLevel()
    {
        int level = GetTargetLevel();

        SpriteShapeController ssc = GetComponent<SpriteShapeController>();
        if (ssc == null) return;

        // Apply track geometry based on level
        switch (level)
        {
            case 2:
                BuildLevel2(ssc);
                break;
            case 3:
                BuildLevel3(ssc);
                break;
            default:
                BuildLevel1(ssc);
                break;
        }

        // Bake the new collider geometry
        ssc.BakeCollider();
    }

    private void BuildLevel1(SpriteShapeController ssc)
    {
        Spline spline = ssc.spline;
        spline.Clear();

        // Level 1: "Green Valley" - Gentle slopes and small fun jumps
        AddPoint(spline, 0, new Vector2(-25f, -8f), Vector2.zero, Vector2.zero, ShapeTangentMode.Linear);
        AddPoint(spline, 1, new Vector2(-25f, 10f), new Vector2(-2f, 1f), new Vector2(4f, -2f), ShapeTangentMode.Continuous);
        AddPoint(spline, 2, new Vector2(-12f, 4f), new Vector2(-4f, 1f), new Vector2(5f, -1.5f), ShapeTangentMode.Continuous);
        AddPoint(spline, 3, new Vector2(5f, 0.5f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 4, new Vector2(30f, 5f), new Vector2(-6f, -1f), new Vector2(6f, -1f), ShapeTangentMode.Continuous);
        AddPoint(spline, 5, new Vector2(55f, 1f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 6, new Vector2(80f, 8f), new Vector2(-6f, -2f), new Vector2(6f, 1.5f), ShapeTangentMode.Continuous);
        AddPoint(spline, 7, new Vector2(105f, 1.5f), new Vector2(-6f, 1.5f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 8, new Vector2(130f, 2f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 9, new Vector2(155f, 7f), new Vector2(-6f, -1.5f), new Vector2(6f, 1f), ShapeTangentMode.Continuous);
        AddPoint(spline, 10, new Vector2(180f, 2f), new Vector2(-6f, 1f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 11, new Vector2(210f, 2f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 12, new Vector2(235f, 2f), new Vector2(-4f, 0f), Vector2.zero, ShapeTangentMode.Continuous);
        AddPoint(spline, 13, new Vector2(235f, -8f), Vector2.zero, Vector2.zero, ShapeTangentMode.Linear);

        ConfigureEnvironment(22f, new Vector3(-20f, 8.2f, 0f), -15f, new Vector3(225f, 3.2f, 0f));
        SpawnCollectiblesAndPowerUps(1);
    }

    private void BuildLevel2(SpriteShapeController ssc)
    {
        Spline spline = ssc.spline;
        spline.Clear();

        // Level 2: "Alpine Canyon" - High launch kicker ramps over canyon drops
        AddPoint(spline, 0, new Vector2(-25f, -10f), Vector2.zero, Vector2.zero, ShapeTangentMode.Linear);
        AddPoint(spline, 1, new Vector2(-26.14f, 14.4f), new Vector2(-2f, 2f), new Vector2(4f, -4f), ShapeTangentMode.Continuous);
        AddPoint(spline, 2, new Vector2(-10f, 7f), new Vector2(-5f, 2.5f), new Vector2(6f, -3f), ShapeTangentMode.Continuous);
        AddPoint(spline, 3, new Vector2(10f, 1f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 4, new Vector2(35f, 7f), new Vector2(-6f, -1.5f), new Vector2(6f, 1.5f), ShapeTangentMode.Continuous);
        AddPoint(spline, 5, new Vector2(55f, 3f), new Vector2(-5f, 1f), new Vector2(6f, -1f), ShapeTangentMode.Continuous);
        // Mega Kicker Ramp 1
        AddPoint(spline, 6, new Vector2(75f, 16f), new Vector2(-6f, -3.5f), new Vector2(6f, 4f), ShapeTangentMode.Continuous);
        // Deep Canyon Gap
        AddPoint(spline, 7, new Vector2(98f, -2f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        // Canyon Landing Slope
        AddPoint(spline, 8, new Vector2(120f, 7f), new Vector2(-6f, -2f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 9, new Vector2(145f, 3f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        // Kicker Ramp 2
        AddPoint(spline, 10, new Vector2(170f, 14f), new Vector2(-6f, -3f), new Vector2(6f, 3.5f), ShapeTangentMode.Continuous);
        AddPoint(spline, 11, new Vector2(195f, 4f), new Vector2(-6f, 2f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 12, new Vector2(225f, 3f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 13, new Vector2(255f, 3f), new Vector2(-4f, 0f), Vector2.zero, ShapeTangentMode.Continuous);
        AddPoint(spline, 14, new Vector2(255f, -10f), Vector2.zero, Vector2.zero, ShapeTangentMode.Linear);

        ConfigureEnvironment(27f, new Vector3(-20f, 12.0f, 0f), -15f, new Vector3(245f, 4.2f, 0f));
        SpawnCollectiblesAndPowerUps(2);
    }

    private void BuildLevel3(SpriteShapeController ssc)
    {
        Spline spline = ssc.spline;
        spline.Clear();

        // Level 3: "Blizzard Peak" - High speed roller-coaster with moguls and mega launcher
        AddPoint(spline, 0, new Vector2(-25f, -12f), Vector2.zero, Vector2.zero, ShapeTangentMode.Linear);
        AddPoint(spline, 1, new Vector2(-25f, 20f), new Vector2(-2f, 1f), new Vector2(5f, -2.5f), ShapeTangentMode.Continuous);
        AddPoint(spline, 2, new Vector2(-10f, 11f), new Vector2(-5f, 3f), new Vector2(6f, -4f), ShapeTangentMode.Continuous);
        AddPoint(spline, 3, new Vector2(10f, 2f), new Vector2(-5f, 0f), new Vector2(5f, 0f), ShapeTangentMode.Continuous);
        // Camel Hump 1
        AddPoint(spline, 4, new Vector2(28f, 11f), new Vector2(-5f, -2f), new Vector2(5f, -2f), ShapeTangentMode.Continuous);
        AddPoint(spline, 5, new Vector2(44f, 3f), new Vector2(-4f, 2f), new Vector2(4f, 2f), ShapeTangentMode.Continuous);
        // Camel Hump 2
        AddPoint(spline, 6, new Vector2(62f, 12f), new Vector2(-5f, -2f), new Vector2(5f, -2f), ShapeTangentMode.Continuous);
        AddPoint(spline, 7, new Vector2(78f, 2.5f), new Vector2(-4f, 2f), new Vector2(5f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 8, new Vector2(100f, 3.5f), new Vector2(-6f, 0f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        // Super Launcher Ramp
        AddPoint(spline, 9, new Vector2(125f, 20f), new Vector2(-7f, -4.5f), new Vector2(7f, 5f), ShapeTangentMode.Continuous);
        // Deep Canyon landing valley below
        AddPoint(spline, 10, new Vector2(158f, 2f), new Vector2(-8f, 0f), new Vector2(8f, 0f), ShapeTangentMode.Continuous);
        // Step Landing Plateau
        AddPoint(spline, 11, new Vector2(185f, 9f), new Vector2(-6f, -2f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        // Half-Pipe wave
        AddPoint(spline, 12, new Vector2(205f, 2.5f), new Vector2(-5f, 2f), new Vector2(5f, 2f), ShapeTangentMode.Continuous);
        AddPoint(spline, 13, new Vector2(230f, 13f), new Vector2(-6f, -3f), new Vector2(6f, 3f), ShapeTangentMode.Continuous);
        AddPoint(spline, 14, new Vector2(255f, 3.5f), new Vector2(-6f, 2f), new Vector2(6f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 15, new Vector2(275f, 3f), new Vector2(-5f, 0f), new Vector2(5f, 0f), ShapeTangentMode.Continuous);
        AddPoint(spline, 16, new Vector2(295f, 3f), new Vector2(-4f, 0f), Vector2.zero, ShapeTangentMode.Continuous);
        AddPoint(spline, 17, new Vector2(295f, -12f), Vector2.zero, Vector2.zero, ShapeTangentMode.Linear);

        ConfigureEnvironment(32f, new Vector3(-20f, 16.7f, 0f), -18f, new Vector3(285f, 4.2f, 0f));
        SpawnCollectiblesAndPowerUps(3);
    }

    private void AddPoint(Spline spline, int index, Vector2 pos, Vector2 leftTangent, Vector2 rightTangent, ShapeTangentMode mode)
    {
        spline.InsertPointAt(index, new Vector3(pos.x, pos.y, 0f));
        spline.SetTangentMode(index, mode);
        if (mode != ShapeTangentMode.Linear)
        {
            spline.SetLeftTangent(index, new Vector3(leftTangent.x, leftTangent.y, 0f));
            spline.SetRightTangent(index, new Vector3(rightTangent.x, rightTangent.y, 0f));
        }
    }

    private void ConfigureEnvironment(float surfaceSpeed, Vector3 playerSpawn, float playerAngle, Vector3 finishPos)
    {
        // 1. Surface Effector
        SurfaceEffector2D surfaceEffector = GetComponent<SurfaceEffector2D>();
        if (surfaceEffector != null)
        {
            surfaceEffector.speed = surfaceSpeed;
            surfaceEffector.useContactForce = false;
            surfaceEffector.useBounce = false;
            surfaceEffector.useFriction = true;
        }

        // 2. Player
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.transform.position = playerSpawn;
            player.transform.rotation = Quaternion.Euler(0f, 0f, playerAngle);
            Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
            if (prb != null)
            {
                prb.linearVelocity = Vector2.zero;
                prb.angularVelocity = 0f;
            }
        }

        // 3. Cinemachine / Main Camera
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(playerSpawn.x, playerSpawn.y + 4f, -10f);
        }

        // 4. Finish Line
        FinishLine finish = FindAnyObjectByType<FinishLine>();
        if (finish != null)
        {
            finish.transform.position = finishPos;
        }
    }

    private void SpawnCollectiblesAndPowerUps(int level)
    {
        // Clean existing spawned dynamic items container
        Transform container = transform.Find("LevelItems");
        if (container != null)
        {
            if (Application.isPlaying)
            {
                Destroy(container.gameObject);
            }
            else
            {
                DestroyImmediate(container.gameObject);
            }
        }

        GameObject itemsRoot = new GameObject("LevelItems");
        itemsRoot.transform.SetParent(transform);

        // Load ScriptableObjects from Resources or Assets if not assigned in inspector
        if (shieldData == null)
        {
            shieldData = Resources.Load<PowerUpScriptableObject>("PowerUpShield");
        }

        // Spawn items depending on level layout
        if (level == 1)
        {
            // Collectibles on hills and jumps
            CreateCollectible(itemsRoot.transform, new Vector3(30f, 7.5f, 0f));
            CreatePowerUp(itemsRoot.transform, new Vector3(55f, 3.5f, 0f), "Speed");
            CreateCollectible(itemsRoot.transform, new Vector3(92f, 11f, 0f));
            CreatePowerUp(itemsRoot.transform, new Vector3(130f, 4.5f, 0f), "Shield");
            CreateCollectible(itemsRoot.transform, new Vector3(155f, 9.5f, 0f));
        }
        else if (level == 2)
        {
            // Blue Shield before the mega kicker
            CreatePowerUp(itemsRoot.transform, new Vector3(45f, 5.5f, 0f), "Shield");
            // Battery in valley
            CreatePowerUp(itemsRoot.transform, new Vector3(140f, 4.5f, 0f), "Speed");

            // Arc of collectibles over the canyon gap
            CreateCollectible(itemsRoot.transform, new Vector3(82f, 16f, 0f));
            CreateCollectible(itemsRoot.transform, new Vector3(92f, 18.5f, 0f));
            CreateCollectible(itemsRoot.transform, new Vector3(102f, 15f, 0f));
            CreateCollectible(itemsRoot.transform, new Vector3(180f, 16f, 0f));
        }
        else if (level == 3)
        {
            // Blue Shield on Camel Hump 1
            CreatePowerUp(itemsRoot.transform, new Vector3(28f, 13.5f, 0f), "Shield");
            // Battery before the Super Launcher
            CreatePowerUp(itemsRoot.transform, new Vector3(95f, 5.5f, 0f), "Speed");
            // Rocket Launcher right at the lip of the Super Launcher!
            CreatePowerUp(itemsRoot.transform, new Vector3(120f, 18.5f, 0f), "Rocket");
            // Blue Shield for step landing plateau
            CreatePowerUp(itemsRoot.transform, new Vector3(185f, 11.5f, 0f), "Shield");

            // Floating collectibles along aerial flight arcs
            CreateCollectible(itemsRoot.transform, new Vector3(44f, 8f, 0f));
            CreateCollectible(itemsRoot.transform, new Vector3(135f, 24f, 0f));
            CreateCollectible(itemsRoot.transform, new Vector3(145f, 26f, 0f));
            CreateCollectible(itemsRoot.transform, new Vector3(155f, 23f, 0f));
            CreateCollectible(itemsRoot.transform, new Vector3(230f, 16.5f, 0f));
        }
    }

    private void CreateCollectible(Transform parent, Vector3 pos)
    {
        GameObject obj;
        if (collectiblePrefab != null)
        {
            obj = Instantiate(collectiblePrefab, pos, Quaternion.identity, parent);
        }
        else
        {
            obj = new GameObject("CollectibleGift");
            obj.transform.SetParent(parent);
            obj.transform.position = pos;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("GiftBag");
            sr.color = new Color(1f, 0.85f, 0.2f); // Golden gift
            sr.sortingOrder = 2;

            CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.9f;

            obj.AddComponent<Collectible>();
        }
    }

    private void CreatePowerUp(Transform parent, Vector3 pos, string type)
    {
        GameObject obj = new GameObject("PowerUp_" + type);
        obj.transform.SetParent(parent);
        obj.transform.position = pos;
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 1f);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 5;

        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(9f, 9f);

        PowerUpManager pum = obj.AddComponent<PowerUpManager>();

        if (type == "Shield")
        {
            Sprite shieldSprite = Resources.Load<Sprite>("PowerUpShieldBlue") ?? Resources.Load<Sprite>("GiftBag");
            sr.sprite = shieldSprite;
            sr.color = Color.white;

            if (shieldData != null)
            {
                pum.Init(shieldData);
            }
            else
            {
                PowerUpScriptableObject s = Resources.Load<PowerUpScriptableObject>("PowerUpShield");
                if (s == null)
                {
                    s = ScriptableObject.CreateInstance<PowerUpScriptableObject>();
                    s.PowerUpType = "Shield";
                    s.PowerUpValue = 1f;
                    s.TimeLimit = 20f;
                }
                pum.Init(s);
            }
        }
        else if (type == "Rocket" || type == "Launcher")
        {
            Sprite rocketSprite = Resources.Load<Sprite>("LauncherRocket") ?? Resources.Load<Sprite>("GiftBag");
            sr.sprite = rocketSprite;
            sr.color = Color.white;

            PowerUpScriptableObject rk = Resources.Load<PowerUpScriptableObject>("PowerUpRocket");
            if (rk == null)
            {
                rk = ScriptableObject.CreateInstance<PowerUpScriptableObject>();
                rk.PowerUpType = "Rocket";
                rk.PowerUpValue = 28f;
                rk.TimeLimit = 3f;
            }
            pum.Init(rk);
        }
        else // Speed / Boost / Battery
        {
            Sprite batterySprite = Resources.Load<Sprite>("PowerUpBattery") ?? Resources.Load<Sprite>("GiftBag");
            sr.sprite = batterySprite;
            sr.color = Color.white;

            if (speedShortData != null)
            {
                pum.Init(speedShortData);
            }
            else
            {
                PowerUpScriptableObject s = Resources.Load<PowerUpScriptableObject>("PowerUpShortSpeed");
                if (s == null)
                {
                    s = ScriptableObject.CreateInstance<PowerUpScriptableObject>();
                    s.PowerUpType = "Speed";
                    s.PowerUpValue = 15f;
                    s.TimeLimit = 4f;
                }
                pum.Init(s);
            }
        }
    }
}
