using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FMODUnity;

public class ZombieJumpScare : MonoBehaviour
{
    [Header("Zombie Reference")]
    [Tooltip("MUST be a GameObject in your Hierarchy scene")]
    [SerializeField] private GameObject zombieObject;

    [Header("Scene Transition Settings")]
    [Tooltip("Exact name of the scene to load upon impact")]
    [SerializeField] private string nextSceneName = "MainGameplay";

    [Tooltip("Brief pause in seconds during the impact jump scare before loading the scene")]
    [SerializeField] private float impactDelayBeforeSceneLoad = 0.2f;

    [Header("Movement & Path Settings")]
    [SerializeField] private float roadSpeed = 25f;
    [SerializeField] private float startX = -12f;
    [SerializeField] private float roadCenterX = 0f;
    [SerializeField] private float spawnZ = 45f;
    [SerializeField] private float leapTriggerZ = 20f;

    [Tooltip("Z position right at the windshield where collision occurs")]
    [SerializeField] private float scareImpactZ = 4f;

    [SerializeField] private float destroyZ = -10f;
    [SerializeField] private float yHeightOffset = 0f;

    [Header("Audio")]
    [SerializeField] private EventReference scareAudioEvent;
    [SerializeField] private EventReference crashImpactAudioEvent;

    private bool isScaring = false;
    private bool hasCollided = false;

    private void Start()
    {
        if (zombieObject == null)
        {
            Debug.LogError("[ZombieJumpScare] No Zombie Object assigned in the Inspector!");
        }
        else
        {
            zombieObject.SetActive(false);
        }
    }

    public void TriggerJumpScare()
    {
        if (isScaring || zombieObject == null) return;

        zombieObject.transform.position = new Vector3(startX, yHeightOffset, spawnZ);
        zombieObject.SetActive(true);

        if (!scareAudioEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(scareAudioEvent);
        }

        StartCoroutine(AnimateJumpScare());
    }

    private IEnumerator AnimateJumpScare()
    {
        isScaring = true;
        Transform zTransform = zombieObject.transform;

        while (zTransform != null && zTransform.position.z > destroyZ && !hasCollided)
        {
            float currentZ = zTransform.position.z - (roadSpeed * Time.deltaTime);
            float currentX = startX;

            if (currentZ <= leapTriggerZ)
            {
                float t = Mathf.InverseLerp(leapTriggerZ, scareImpactZ, currentZ);
                currentX = Mathf.Lerp(startX, roadCenterX, t);
            }

            zTransform.position = new Vector3(currentX, yHeightOffset, currentZ);

            // Trigger collision when zombie reaches windshield position
            if (currentZ <= scareImpactZ && !hasCollided)
            {
                OnCarImpact();
                yield break;
            }

            yield return null;
        }
    }

    // Optional: Physical collider fallback if car has a Trigger Collider tagged "Player"
    private void OnTriggerEnter(Collider other)
    {
        if (!hasCollided && (other.CompareTag("Player") || other.name.ToLower().Contains("car")))
        {
            OnCarImpact();
        }
    }

    private void OnCarImpact()
    {
        if (hasCollided) return;
        hasCollided = true;

        // Play impact sound effect
        if (!crashImpactAudioEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(crashImpactAudioEvent);
        }

        // Save FMOD master volume state before scene load
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SaveVolumeSettings();
        }

        StartCoroutine(LoadNextSceneRoutine());
    }

    private IEnumerator LoadNextSceneRoutine()
    {
        yield return new WaitForSeconds(impactDelayBeforeSceneLoad);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("[ZombieJumpScare] Next Scene Name is blank! Specify scene name in Inspector.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 spawnPoint = new Vector3(startX, yHeightOffset, spawnZ);
        Vector3 leapPoint = new Vector3(startX, yHeightOffset, leapTriggerZ);
        Vector3 impactPoint = new Vector3(roadCenterX, yHeightOffset, scareImpactZ);
        Vector3 endPoint = new Vector3(roadCenterX, yHeightOffset, destroyZ);

        Gizmos.DrawWireSphere(spawnPoint, 0.8f);
        Gizmos.DrawLine(spawnPoint, leapPoint);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(leapPoint, impactPoint);
        Gizmos.DrawLine(impactPoint, endPoint);
        Gizmos.DrawWireSphere(impactPoint, 1f);
    }
}