using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    // Tunables
    [SerializeField] float delayBetweenMovements = 0.5f;

    // State
    Waypoint startWaypoint = null;
    Waypoint endWaypoint = null;

    // Cached references
    PathFinder pathfinder = null;

    public void Nudge()
    {
        pathfinder = GetComponent<PathFinder>();
        pathfinder.SetStartEndWaypoints(startWaypoint, endWaypoint);
        List<Waypoint> startToEndPath = new List<Waypoint>(pathfinder.CreateAndGetPath());
        StartCoroutine(MoveAlongPath(startToEndPath));
    }

    private IEnumerator MoveAlongPath(List<Waypoint> path)
    {
        foreach (Waypoint block in path)
        {
            Vector3 currentPosition = block.GetGridPosition();
            transform.position = currentPosition;
            yield return new WaitForSeconds(delayBetweenMovements);
        }
        TriggerGoalExplosion();
    }

    private void TriggerGoalExplosion()
    {
        FindObjectOfType<PlayerProperties>().DecrementHealth();
        Enemy enemy = GetComponent<Enemy>();
        enemy.TriggerDeathSequence(enemy.LARGE_EXPLOSION);
    }

    public void SetStartEndWaypoints(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
    }

    public Waypoint GetStartWaypoint()
    {
        return startWaypoint;
    }

    public Waypoint GetEndWaypoint()
    {
        return endWaypoint;
    }
}
