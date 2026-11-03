using UnityEngine;
using System.Collections;

public class PassingSignController : MonoBehaviour
{
    [Header("Movement Path")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(3.5f, 1.2f, 60f);
    [SerializeField] private Vector3 readPosition = new Vector3(3.5f, 1.2f, 12f);
    [SerializeField] private Vector3 passPosition = new Vector3(3.5f, 1.2f, -15f);

    [Header("Timing & Speed")]
    [SerializeField] private float approachDuration = 3.5f; // Time to reach the front window
    [SerializeField] private float passByDuration = 0.8f;   // Time to zip past the car
    [SerializeField] private AnimationCurve approachCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Behavior")]
    [SerializeField] private bool autoStartOnLaunch = true;

    private void Start()
    {
        transform.position = spawnPosition;

        if (autoStartOnLaunch)
        {
            TriggerSignPass();
        }
    }

    public void TriggerSignPass()
    {
        StartCoroutine(AnimateSignMotion());
    }

    private IEnumerator AnimateSignMotion()
    {
        // 1. Approach Phase: Move from far away into the readable window frame
        float elapsed = 0f;
        while (elapsed < approachDuration)
        {
            elapsed += Time.deltaTime;
            float t = approachCurve.Evaluate(elapsed / approachDuration);
            transform.position = Vector3.Lerp(spawnPosition, readPosition, t);
            yield return null;
        }

        // 2. Zip-By Phase: Accelerate quickly past the player's view
        elapsed = 0f;
        while (elapsed < passByDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / passByDuration;
            // Quadratic acceleration (t * t) gives that illusion of high relative speed
            transform.position = Vector3.Lerp(readPosition, passPosition, t * t);
            yield return null;
        }

        transform.position = passPosition;
    }

    // Visualization gizmos in Scene View to easily position points
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPosition, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(readPosition, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(passPosition, 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(spawnPosition, readPosition);
        Gizmos.DrawLine(readPosition, passPosition);
    }
}