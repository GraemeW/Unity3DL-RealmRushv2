using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // STATIC
    private static string LAYER_ENEMY = "Enemy"; // string reference

    // Public variables
    public bool SMALL_EXPLOSION = false;
    public bool LARGE_EXPLOSION = true;

    // Tunables
    [Header("Enemy Properties")]
    [SerializeField] float maxBaseHitPoints = 10f;
    [SerializeField] float hitPointJitter = 5f;
    [SerializeField] ParticleSystem enemyHitVFX = null;
    [SerializeField] AudioClip enemyHitSFX = null;
    [Header("Death FX")]
    [SerializeField] GameObject explosion = null;
    [SerializeField] float explosionTime = 0.7f;
    [SerializeField] GameObject factoryExplosion = null;
    [SerializeField] float factoryExplosionTime = 5f;

    // State
    float currentHitPoints = 10;

    // Cached Reference
    AudioSource audioSource = null;

    private void Start()
    {
        SetHitPoints();
        audioSource = GetComponent<AudioSource>();
    }

    private void SetHitPoints()
    {
        currentHitPoints = UnityEngine.Random.Range(maxBaseHitPoints, maxBaseHitPoints + hitPointJitter);
    }

    private void OnParticleCollision(GameObject other)
    {
        currentHitPoints--;
        TriggerHitFX();
        if (currentHitPoints <= 0)
        {
            TriggerDeathSequence(SMALL_EXPLOSION);
        }
    }

    private void TriggerHitFX()
    {
        enemyHitVFX.Play();
        audioSource.PlayOneShot(enemyHitSFX);
    }

    public void TriggerDeathSequence(bool largeExplosion)
    {
        GameObject explosionType = explosion;
        float timeToHoldExplosion = explosionTime;
        if (largeExplosion) 
        {
            explosionType = factoryExplosion;
            timeToHoldExplosion = factoryExplosionTime; 
        }

        GameObject deathExplosion = Instantiate(explosionType, transform.position, Quaternion.identity);
        deathExplosion.layer = LayerMask.NameToLayer(LAYER_ENEMY);
        Destroy(deathExplosion, timeToHoldExplosion);
        Destroy(gameObject, timeToHoldExplosion);
        gameObject.SetActive(false);
    }
}
