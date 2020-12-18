using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFactory : MonoBehaviour
{
    // Tunables
    [Header("Tower Spawn Properties")]
    [SerializeField] Tower towerPrefab = null;
    [SerializeField] Transform towerHierarchy = null;

    // State
    int maxNumberTowers = 0;
    int currentNumberTowers = 0;
    LinkedList<Tower> towerQueue = new LinkedList<Tower>();

    // Cached References
    PlayerProperties playerHealth = null;

    void Start()
    {
        CalculateMaxNumberTowers();
        playerHealth = FindObjectOfType<PlayerProperties>();
    }

    private void CalculateMaxNumberTowers()
    {
        FactoryBase[] factories = FindObjectsOfType<FactoryBase>();
        foreach (FactoryBase factory in factories)
        {
            if (factory.GetFriendlyBase())
            {
                maxNumberTowers += factory.GetTowerAdder();
            }
        }
    }

    public void PlaceTower(Waypoint waypoint)
    {
        bool aliveStatus = playerHealth.GetAliveStatus();
        if (towerPrefab != null && aliveStatus)
        {
            if (currentNumberTowers < maxNumberTowers)
            {
                SpawnTower(waypoint);
            }
            else
            {
                MoveTower(waypoint);
            }
        }
    }

    private void MoveTower(Waypoint waypoint)
    {
        Tower popTower = towerQueue.First.Value;
        if (popTower != null)
        {
            popTower.GetBaseWaypoint().isOccupied = false;
            towerQueue.RemoveFirst();
            popTower.transform.position = waypoint.transform.position;
            SetWaypointAndQueue(waypoint, popTower);
        }
    }

    private void SpawnTower(Waypoint waypoint)
    {
        currentNumberTowers++;
        Tower spawnTower = Instantiate(towerPrefab, waypoint.transform.position, Quaternion.identity);
        if (towerHierarchy != null)
        {
            spawnTower.transform.parent = towerHierarchy;
        }
        SetWaypointAndQueue(waypoint, spawnTower);
    }

    private void SetWaypointAndQueue(Waypoint waypoint, Tower tower)
    {
        tower.SetBaseWaypoint(waypoint);
        waypoint.isOccupied = true;
        towerQueue.AddLast(tower);
    }

    public void RemoveTowerFromQueue(Tower tower)
    {
        towerQueue.Remove(tower);
        currentNumberTowers--;
    }
}
