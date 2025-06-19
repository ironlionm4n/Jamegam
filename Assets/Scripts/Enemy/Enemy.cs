using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private GameObject dropPart;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    
    private AudioSource _hitAudioSource;
    private float _attackTimer;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(_hitAudioSource == null)
        {
            _hitAudioSource = GetComponent<AudioSource>();
            if (_hitAudioSource == null)
            {
                Debug.LogError("Enemy requires an AudioSource component for hit and death sounds.");
                return;
            }
        }
        var projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            TakeDamage(projectile);
        }
    }

    private void TakeDamage(Projectile projectile)
    {
        health -= projectile.Damage;
        projectile.DestroySelf();
        _hitAudioSource.clip = hitSound;
        _hitAudioSource.PlayOneShot(_hitAudioSource.clip);
        
        if (health <= 0)
        {
            Instantiate(dropPart, transform.position, Quaternion.identity);
            Die();
        }
    }

    private void Die()
    {
        _hitAudioSource.clip = deathSound;
        _hitAudioSource.PlayOneShot(_hitAudioSource.clip);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, _hitAudioSource.clip.length + .1f);
    }
}
