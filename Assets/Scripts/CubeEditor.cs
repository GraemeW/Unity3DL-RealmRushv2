using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
public class CubeEditor : MonoBehaviour
{
    // Tunables
    [SerializeField] TextMesh positionLabel = null;
    bool labelsOn = false;

    // State
    bool isBase = false;
    bool checkedBase = false;
    bool isTower = false;
    bool checkedTower = false;

    // Cached references
    Waypoint waypoint = null;
    PathFinder pathfinder = null;
    FactoryBase factoryBase = null;
    Tower tower = null;

    private void Awake()
    {
        waypoint = GetComponent<Waypoint>();
        pathfinder = FindObjectOfType<PathFinder>();
        factoryBase = GetComponent<FactoryBase>();
        if (factoryBase != null) { isBase = true; }
        tower = GetComponent<Tower>();
        if (tower != null) { isTower = true; }
    }

    private void Update()
    {
        RefreshCachedReferences();
        SnapToGrid(waypoint);
        if (!isBase && !isTower) { LabelBlock(waypoint); }
    }

    private void RefreshCachedReferences()
    {
        if (!waypoint)
        {
            waypoint = GetComponent<Waypoint>();
        }
        if (!pathfinder)
        {
            pathfinder = FindObjectOfType<PathFinder>();
        }
        if (!factoryBase && !checkedBase)
        {
            factoryBase = GetComponent<FactoryBase>();
            if (factoryBase != null) { isBase = true; }
            checkedBase = true;
        }
        if (!tower && !checkedTower)
        {
            tower = GetComponent<Tower>();
            if (tower != null) { isTower = true; }
            checkedTower = true;
        }
    }

    private void SnapToGrid(Waypoint waypoint)
    {
        Vector3 gridPosition = waypoint.GetGridPosition();
        Vector3 snapPosition = new Vector3(gridPosition.x, 0f, gridPosition.z);
        transform.position = snapPosition;
    }

    private void LabelBlock(Waypoint waypoint)
    {
        Vector3 gridPositionNormalized = waypoint.GetGridPositionNormalized();
        string label = gridPositionNormalized.x + "," + gridPositionNormalized.z;
        gameObject.name = "Cube: " + label;
        if (labelsOn)
        {
            if (positionLabel != null)
            {
                positionLabel.text = label;
            }
        }
        else
        {
            if (positionLabel != null)
            {
                positionLabel.text = "";
            }
        }
    }
}
