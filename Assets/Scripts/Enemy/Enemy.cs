using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float detectionRange;
    [SerializeField] private GameObject dropPart;
    [SerializeField] private float turnRate = 180f; // degrees per second
    
    private float attackTimer;
    
    public float forwardSpeed = 5f;
    public float amplitude = 2f;
    public float freq = 2f;
    float t;

    void FixedUpdate()
    {
        // fly down the screen
        t += Time.deltaTime;
        float yOffset = amplitude * Mathf.Sin(freq * t);
        Vector2 targetPosition = new Vector2(transform.position.x * Time.deltaTime + yOffset, transform.position.y - forwardSpeed * Time.deltaTime);
        transform.position = targetPosition;
    }

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
        
        if (health <= 0)
        {
            Instantiate(dropPart);
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
