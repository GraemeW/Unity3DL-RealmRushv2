using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Constants
    const int gridSize = 10;

    // State
    Vector3 gridPosition;
    Vector3 gridPositionNormalized;
    [Header("Pathfinding Properties")]
    public bool isExplored = false; // data class
    public Waypoint exploredFrom = null; // data class
    // Tunables
    [Header("Block Attributes")]
    public bool isTraversible = true; // false selection on neutral blocks
    public bool isTower = false;
    public bool isEnemyBlock = false;
    public bool isOccupied = false;

    private void Start()
    {
        FactoryBase factoryBase = GetComponent<FactoryBase>();
        Tower tower = GetComponent<Tower>();

        if (factoryBase != null || tower != null)
        {
            isTraversible = false;
        }
        if (tower != null)
        {
            isTower = true;
        }
    }

    public int GetGridSize()
    {
        return gridSize;
    }

    public Vector3 GetGridPositionNormalized()
    {
        gridPositionNormalized = new Vector3
        {
            x = Mathf.RoundToInt(transform.position.x / (float)gridSize),
            z = Mathf.RoundToInt(transform.position.z / (float)gridSize)
        };
        return gridPositionNormalized;
    }

    public Vector3 GetGridPosition()
    {
        Vector3 gridPositionNormalized = GetGridPositionNormalized();
        gridPosition =  new Vector3
        {
            x = (float)gridPositionNormalized.x * (float)gridSize,
            z = (float)gridPositionNormalized.z * (float)gridSize
        };
        return gridPosition;
    }

    private void OnMouseOver()
    {
        if (isTower || !isTraversible || isEnemyBlock || isOccupied) { return; }
        if (Input.GetButtonDown("Fire1"))
        {
            FindObjectOfType<TowerFactory>().PlaceTower(this);
        }
    }
}
