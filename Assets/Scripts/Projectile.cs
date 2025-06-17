using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _projectileRigidbody;
    private float _lifetime = 5f;
    private float _speed = 10f;
    public float Damage { get; set; } = 2f;

    private void Awake()
    {
        _projectileRigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnProjectileSpawned()
    {
        transform.SetParent(null);
        _projectileRigidbody.linearVelocity = transform.up * _speed;
        Destroy(gameObject, _lifetime);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
