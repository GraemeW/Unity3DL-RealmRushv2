using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // STATIC
    static bool GRID_WITHOUT_TOWERS = false;
    static bool GRID_WITH_TOWERS = true;

    // Tunables
    Vector3[] directions =
    {
        new Vector3(0,0,1), // Up
        new Vector3(1,0,0), // Right
        new Vector3(0,0,-1), // Down
        new Vector3(-1,0,0) // Left
    };

    // State
    Waypoint startWaypoint = null;
    Waypoint endWaypoint = null;
    Dictionary<Vector3, Waypoint> grid = null;
    Dictionary<Vector3, Waypoint> gridWithTowers = null;
    Dictionary<Vector3, Waypoint> gridWithoutTowers = null;
    Queue<Waypoint> queue = new Queue<Waypoint>();
    bool isRunning = false;
    Waypoint currentSearchCenter = null;
    List<Waypoint> path = new List<Waypoint>();

    public void SetStartEndWaypoints(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
    }

    public List<Waypoint> CreateAndGetPath()
    {
        ResetBlockState();
        LoadBlocks();
        LoadBlocksWithoutTowers();
        Pathfind(GRID_WITHOUT_TOWERS);
        bool pathExistsWithoutTowers = GeneratePathFromBreadcrumbs();
        if (!pathExistsWithoutTowers)
        {
            ResetBlockState();
            Pathfind(GRID_WITH_TOWERS);
            GeneratePathFromBreadcrumbs();
        }
        return path;
    }

    private void ResetBlockState()
    {
        Waypoint[] waypoints = FindObjectsOfType<Waypoint>();
        foreach (Waypoint waypoint in waypoints)
        {
            waypoint.isExplored = false;
            waypoint.exploredFrom = null;
        }
    }

    private void LoadBlocks()
    {
        gridWithTowers = new Dictionary<Vector3, Waypoint>();
        Waypoint[] waypoints = FindObjectsOfType<Waypoint>();
        foreach (Waypoint waypoint in waypoints)
        {
            if (waypoint.isTraversible && !waypoint.isTower)
            {
                if (!gridWithTowers.ContainsKey(waypoint.GetGridPositionNormalized()))
                {
                    gridWithTowers.Add(waypoint.GetGridPositionNormalized(), waypoint);
                }
                else
                {
                    UnityEngine.Debug.Log("Skipping overlapping block:  " + waypoint);
                }
            }
        }
    }

    private void LoadBlocksWithoutTowers()
    {
        gridWithoutTowers = new Dictionary<Vector3, Waypoint>(gridWithTowers);
        Tower[] towers = FindObjectsOfType<Tower>();
        foreach (Tower tower in towers)
        {
            gridWithoutTowers.Remove(tower.gameObject.GetComponent<Waypoint>().GetGridPositionNormalized());
        }
    }

    private void Pathfind(bool withTowers)
    {
        if (withTowers) { grid = gridWithTowers; }
            else { grid = gridWithoutTowers; }

        isRunning = true;
        queue.Clear();
        queue.Enqueue(startWaypoint);

        while (queue.Count > 0 && isRunning)
        {
            currentSearchCenter = queue.Dequeue();
            HaltIfEndFound();
            ExploreNeighbors();
            currentSearchCenter.isExplored = true;
        }
    }

    private void HaltIfEndFound()
    {
        if (currentSearchCenter == endWaypoint)
        {
            isRunning = false;
        }
    }

    private void ExploreNeighbors()
    {
        if (!isRunning) { return; }

        foreach (Vector3 direction in directions)
        {
            Vector3 checkPosition = currentSearchCenter.GetGridPositionNormalized() + direction;
            if (grid.ContainsKey(checkPosition))
            {
                QueueNewNeighbors(checkPosition);
            }
        }
    }

    private void QueueNewNeighbors(Vector3 checkPosition)
    {
        Waypoint neighbor = grid[checkPosition];

        if (!neighbor.isExplored && !queue.Contains(neighbor))
        {
            queue.Enqueue(neighbor);
            neighbor.exploredFrom = currentSearchCenter;
        }
    }

    private bool GeneratePathFromBreadcrumbs()
    {
        bool pathExists = true; // Behavior (expected):  Enemies teleport to end if no path exists, auto-lose

        Waypoint currentWaypoint = endWaypoint;
        path.Add(currentWaypoint);
        do
        {
            if (currentWaypoint.exploredFrom != null)
            {
                currentWaypoint = currentWaypoint.exploredFrom;
                path.Add(currentWaypoint);
            }
            else { pathExists = false; }
        }
        while (pathExists && currentWaypoint != startWaypoint);
        path.Reverse();
        return pathExists;
    }
}
