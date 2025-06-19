using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private GameObject dropPart;
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    
    private float attackTimer;

    private void OnCollisionEnter2D(Collision2D other)
    {
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
        hitAudioSource.clip = hitSound;
        hitAudioSource.PlayOneShot(hitAudioSource.clip);
        
        if (health <= 0)
        {
            Instantiate(dropPart, transform.position, Quaternion.identity);
            Die();
        }
    }

    private void Die()
    {
        hitAudioSource.clip = deathSound;
        hitAudioSource.PlayOneShot(hitAudioSource.clip);
        GetComponent<SpriteRenderer>().enabled = false; // Hide the enemy sprite
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.5f);
    }
}
