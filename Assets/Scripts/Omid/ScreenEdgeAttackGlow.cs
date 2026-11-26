using UnityEngine;
using UnityEngine.UI;

public class ScreenEdgeAttackGlow : MonoBehaviour
{
    public static ScreenEdgeAttackGlow Instance { get; private set; }

    [Header("Player Camera Reference")]
    [Tooltip("Assign your Main Camera so Left/Right/Front/Back always match where the player is looking")]
    [SerializeField] private Transform playerCamera;

    [Header("Screen-Edge Glow Images")]
    [SerializeField] private Image glowTop;     // Front (in front of camera)
    [SerializeField] private Image glowRight;   // Right of camera
    [SerializeField] private Image glowBottom;  // Behind camera
    [SerializeField] private Image glowLeft;    // Left of camera

    [Header("Pulse & Color Settings")]
    [SerializeField] private Color glowColor = new Color(0.25f, 0.9f, 0.2f, 1f); // Matches your green blur (or change to red)
    [SerializeField] private float maxAlpha = 0.85f;
    [SerializeField] private float fadeOutSpeed = 1.8f;     // How fast the glow fades after an attack stops
    [SerializeField] private float pulseSpeed = 8f;         // Speed of the heartbeat/throb while active
    [SerializeField] private float pulseAmount = 0.2f;      // Intensity of the throb
    [SerializeField] private float pulseScale = 1.12f;      // Slight expansion on each hit

    // Index: 0 = Top (Front), 1 = Right, 2 = Bottom (Rear), 3 = Left
    private Image[] edgeImages;
    private float[] edgeIntensity = new float[4];
    private Vector3[] baseScales = new Vector3[4];

    private void Awake()
    {
        Instance = this;

        edgeImages = new Image[] { glowTop, glowRight, glowBottom, glowLeft };

        for (int i = 0; i < edgeImages.Length; i++)
        {
            if (edgeImages[i] != null)
            {
                baseScales[i] = edgeImages[i].rectTransform.localScale;
                Color c = glowColor;
                c.a = 0f;
                edgeImages[i].color = c;
            }
        }
    }

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        // Smooth sine wave for the rhythmic pulse while a side is under attack
        float pulseWave = 1f - pulseAmount + (Mathf.Sin(Time.time * pulseSpeed) * pulseAmount);

        for (int i = 0; i < edgeImages.Length; i++)
        {
            if (edgeImages[i] == null) continue;

            if (edgeIntensity[i] > 0f)
            {
                edgeIntensity[i] = Mathf.Max(0f, edgeIntensity[i] - fadeOutSpeed * Time.deltaTime);
            }

            float finalAlpha = edgeIntensity[i] * maxAlpha * pulseWave;

            Color c = glowColor;
            c.a = finalAlpha;
            edgeImages[i].color = c;

            // Subtle scale breathe on hit
            float scaleMult = Mathf.Lerp(1f, pulseScale, edgeIntensity[i] * pulseWave);
            edgeImages[i].rectTransform.localScale = baseScales[i] * scaleMult;
        }
    }

    /// <summary>
    /// Call this when a zombie attacks the truck. Automatically lights up Top/Right/Bottom/Left
    /// relative to where the player camera is currently facing.
    /// </summary>
    public void TriggerEdgeGlow(Vector3 zombieWorldPos)
    {
        if (playerCamera == null) return;

        // Convert zombie position into the player camera's local space
        Vector3 localDir = playerCamera.InverseTransformPoint(zombieWorldPos);
        localDir.y = 0f;

        if (localDir.sqrMagnitude < 0.001f) return;

        // Angle in degrees relative to camera forward:
        // 0° = Front (Top), +90° = Right, ±180° = Behind (Bottom), -90° = Left
        float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        int edgeIndex;
        if (angle >= -45f && angle < 45f)
            edgeIndex = 0; // Top (Front)
        else if (angle >= 45f && angle < 135f)
            edgeIndex = 1; // Right
        else if (angle >= -135f && angle < -45f)
            edgeIndex = 3; // Left
        else
            edgeIndex = 2; // Bottom (Behind)

        edgeIntensity[edgeIndex] = 1f;
    }
}