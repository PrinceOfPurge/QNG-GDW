using UnityEngine;

public class RadioVolumeKnob3D : MonoBehaviour
{
    public enum InteractionType
    {
        RadialSpin,     // Orbit mouse clockwise/counter-clockwise around dial center
        LinearDrag      // Drag mouse Up/Right (Volume Up) or Down/Left (Volume Down)
    }

    [Header("Interaction & Direction")]
    [SerializeField] private InteractionType interactionMode = InteractionType.RadialSpin;

    [Tooltip("Check this if your local 3D mesh facing rotates backward relative to the camera")]
    [SerializeField] private bool invertDirection = false;

    [Header("Z-Axis Limits")]
    [Tooltip("Angle offset at 0 Volume")]
    [SerializeField] private float minZAngle = -360f;

    [Tooltip("Angle offset at 1 (Max) Volume")]
    [SerializeField] private float maxZAngle = 360f;

    [Header("Tactile Feel & Inertia")]
    [Tooltip("Inertia speed. Lower values (10-15) feel heavier; higher values (25-30) feel snappier.")]
    [SerializeField] private float smoothingSpeed = 18f;

    [Tooltip("Sensitivity multiplier when using Linear Drag mode")]
    [SerializeField] private float linearSensitivity = 1.5f;

    [Header("Camera Reference")]
    [Tooltip("Leave blank to automatically pick up Main Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("State")]
    [Range(0f, 1f)]
    [SerializeField] private float currentVolume = 1f;

    private float targetZAngle;
    private float displayedZAngle;
    private float initialMouseAngle;
    private float initialKnobZAngle;
    private Vector3 previousMousePos;
    private Vector3 baseLocalEuler;
    private bool isDragging = false;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        // Cache base local angles to protect X and Y orientation
        baseLocalEuler = transform.localEulerAngles;

        // Sync initial volume from AudioManager
        if (AudioManager.instance != null)
        {
            currentVolume = AudioManager.instance.masterVolume;
        }

        // Set initial Z target angle based on volume
        targetZAngle = Mathf.Lerp(minZAngle, maxZAngle, currentVolume);
        displayedZAngle = targetZAngle;
        ApplyZRotation(displayedZAngle);
    }

    private void OnMouseDown()
    {
        isDragging = true;
        previousMousePos = Input.mousePosition;
        initialMouseAngle = GetMouseAngleOnScreen();
        initialKnobZAngle = targetZAngle;
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        if (interactionMode == InteractionType.RadialSpin)
        {
            float currentMouseAngle = GetMouseAngleOnScreen();

            // Reversed subtraction (initial - current) converts clockwise cursor motion into positive delta
            float angleDelta = Mathf.DeltaAngle(currentMouseAngle, initialMouseAngle);

            if (invertDirection) angleDelta = -angleDelta;

            targetZAngle = Mathf.Clamp(initialKnobZAngle + angleDelta, minZAngle, maxZAngle);
        }
        else // Linear Drag
        {
            Vector3 mouseDelta = Input.mousePosition - previousMousePos;
            previousMousePos = Input.mousePosition;

            float dragAmount = (mouseDelta.x + mouseDelta.y) * linearSensitivity;
            if (invertDirection) dragAmount = -dragAmount;

            targetZAngle = Mathf.Clamp(targetZAngle + dragAmount, minZAngle, maxZAngle);
        }

        // Calculate 0.0 to 1.0 volume ratio
        currentVolume = Mathf.InverseLerp(minZAngle, maxZAngle, targetZAngle);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.masterVolume = currentVolume;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void Update()
    {
        // Interpolate displayed rotation towards target rotation for physical weight feel
        if (Mathf.Abs(displayedZAngle - targetZAngle) > 0.001f)
        {
            displayedZAngle = Mathf.Lerp(displayedZAngle, targetZAngle, Time.deltaTime * smoothingSpeed);
            ApplyZRotation(displayedZAngle);
        }
    }

    private float GetMouseAngleOnScreen()
    {
        if (targetCamera == null) return 0f;

        Vector3 knobScreenPos = targetCamera.WorldToScreenPoint(transform.position);
        Vector3 mouseDirection = Input.mousePosition - knobScreenPos;

        return Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
    }

    private void ApplyZRotation(float zAngle)
    {
        transform.localEulerAngles = new Vector3(
            baseLocalEuler.x,
            baseLocalEuler.y,
            baseLocalEuler.z + zAngle
        );
    }
}