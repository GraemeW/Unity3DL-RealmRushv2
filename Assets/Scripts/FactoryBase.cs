using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryBase : MonoBehaviour
{
    // Tunables
    [Header("Spawn & End Point")]
    [SerializeField] Waypoint startWaypoint = null;
    [SerializeField] Waypoint endWaypoint = null;
    [Header("Spawn Details")]
    [SerializeField] bool enemyBase = true;
    [SerializeField] float spawnPeriod = 5.0f;
    [SerializeField] GameObject enemyPrefab = null;
    [SerializeField] Transform enemyHierarchy = null;
    [Header("Tower Properties")]
    [SerializeField] float healthAdder = 5f;
    [SerializeField] bool friendlyBase = false;
    [SerializeField] int numberTowers = 3;

    [Header("State Properties")]
    // State
    [SerializeField] bool isSpawning = true;

    private void Start()
    {
        if (enemyBase)
        {
            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy()
    {
        while (isSpawning)
        {
            GameObject spawn = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            SetUpEnemyMovement(spawn);
            FindObjectOfType<PlayerProperties>().IncrementScore();
            yield return new WaitForSeconds(spawnPeriod);
        }
    }

    private void SetUpEnemyMovement(GameObject spawn)
    {
        EnemyMovement spawnEnemyMovement = spawn.GetComponent<EnemyMovement>();
        spawnEnemyMovement.SetStartEndWaypoints(startWaypoint, endWaypoint);
        spawnEnemyMovement.Nudge();
        spawn.transform.parent = enemyHierarchy;
    }

    public bool GetFriendlyBase()
    {
        return friendlyBase;
    }

    public int GetTowerAdder()
    {
        return numberTowers;
    }

    public float GetHealthAdder()
    {
        return healthAdder;
    }
}
