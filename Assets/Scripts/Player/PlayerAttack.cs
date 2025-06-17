using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileSpawnPoint_left;
    [SerializeField] private GameObject projectileSpawnPoint_right;
    [SerializeField, Tooltip("Time in between attacks")] private float fireRate = 0.5f;
    
    private InputSystem_Actions _inputActions;
    private int _spawnerIndex; // 0 for left, 1 for right
    private float _nextFireTime;
    private bool _canAttack;

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();
        }
    }

    private void Update()
    {
        if (_inputActions == null)
        {
            Debug.LogWarning("Input actions are not initialized.");
            return;
        }

        if (_inputActions.Player.Attack.IsPressed() && _canAttack && Time.time >= _nextFireTime)
        {
            Attack();
            _canAttack = true; // Reset attack availability
        }

        if (Time.time >= _nextFireTime)
        {
            _canAttack = true;
        }      
    }

    private void Attack()
    {
        _canAttack = false;
        _nextFireTime = Time.time + fireRate;
        var currentSpawner = _spawnerIndex == 0 ? projectileSpawnPoint_left : projectileSpawnPoint_right;
        switch (_spawnerIndex)
        {
            case 0:
                SpawnProjectile(currentSpawner);
                _spawnerIndex = 1; // Switch to right
                break;
            case 1:
                SpawnProjectile(currentSpawner);
                _spawnerIndex = 0; // Switch to left
                break;
        }
    }

    private void SpawnProjectile(GameObject projectileSpawnPointLeft)
    {
        if (projectilePrefab != null && projectileSpawnPointLeft != null)
        {
            var spawnedProjectile = Instantiate(projectilePrefab, projectileSpawnPointLeft.transform.position, Quaternion.identity);
            var projectile = spawnedProjectile.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.OnProjectileSpawned(); // Ensure the projectile is not a child of the player
            }
            else
            {
                Debug.LogWarning("Spawned projectile does not have a Projectile component.");
            }
        }
        else
        {
            Debug.LogWarning("Projectile prefab or spawn point is not set.");
        }
    }
}
