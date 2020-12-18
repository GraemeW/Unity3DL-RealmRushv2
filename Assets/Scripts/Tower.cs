using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // Tunables
    [Header("Gun Controls")]
    [SerializeField] Transform objectToPan = null;
    [SerializeField] float enemyYOffset = 2.0f;
    [SerializeField] float targetingRange = 100.0f;
    [SerializeField] ParticleSystem gun = null;
    [SerializeField] AudioClip gunSound = null;
    [SerializeField] float gunFiringPeriod = 0.67f;
    [Header("Tower Properties")]
    [SerializeField] GameObject explosion = null;
    [SerializeField] float explosionTime = 0.7f;

    // State
    Enemy targetEnemy = null;
    bool isShooting = false;
    Waypoint baseWaypoint = null;

    // Cached Reference
    AudioSource audioSource = null;
    TowerFactory towerFactory = null;

    private void Start()
    {
        SetUpGuns();
        towerFactory = FindObjectOfType<TowerFactory>();
    }

    private void SetUpGuns()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = gunSound;
        gun.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            TriggerDeathSequence();
        }
    }

    private void TriggerDeathSequence()
    {
        towerFactory.RemoveTowerFromQueue(this);
        baseWaypoint.isOccupied = false;

        GameObject deathExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(deathExplosion, explosionTime);
        Destroy(gameObject, explosionTime);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        FindNearestEnemy();
        Shoot();
    }

    private void FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        if (enemies.Length > 0)
        {
            float lastEnemyDistance = Mathf.Infinity;
            foreach (Enemy enemy in enemies)
            {
                float currentEnemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
                if (currentEnemyDistance < lastEnemyDistance)
                {
                    targetEnemy = enemy;
                    lastEnemyDistance = currentEnemyDistance;
                }
            }
        }
        else
        {
            targetEnemy = null;
        }
    }

    private void Shoot()
    {
        if (targetEnemy)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distanceToEnemy<targetingRange)
            {
                objectToPan.LookAt(targetEnemy.transform.position + Vector3.up * enemyYOffset);
                if (!isShooting)
                {
                    isShooting = true;
                    gun.Play();
                    StartCoroutine(InitiateGunSounds());
                }
            }
            else
            {
                StopShooting(); // Stop if out of range
            }
        }
        else
        {
            StopShooting(); // Stop if no enemy on map
        }
    }

    private void StopShooting()
    {
        if (isShooting)
        {
            isShooting = false;
            gun.Stop();
        }
    }

    private IEnumerator InitiateGunSounds()
    {
        while (isShooting)
        {
            audioSource.Play();
            yield return new WaitForSeconds(gunFiringPeriod);
        }
    }

    public void SetBaseWaypoint(Waypoint waypoint)
    {
        baseWaypoint = waypoint;
    }

    public Waypoint GetBaseWaypoint()
    {
        return baseWaypoint;
    }
}
