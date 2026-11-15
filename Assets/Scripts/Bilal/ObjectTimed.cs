using System;
using UnityEngine;

// Lives for 'lifetime' seconds, then either returns itself to a pool
// (pooling on) or destroys itself (pooling off).
public class TimedObject : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;

    // Set by the spawner when pooling is on. If it is null, the object destroys itself.
    public Action<TimedObject> OnExpired;

    private float _timer;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Runs every time the object is created OR taken back out of the pool,
    // so this is where a reused object gets "reset".
    private void OnEnable()
    {
        _timer = lifetime;

        if (_rb != null) // clear leftover motion from the previous use
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        if (OnExpired != null) OnExpired(this); // pooled: disable and recycle
        else Destroy(gameObject);               // not pooled: destroy
    }
}