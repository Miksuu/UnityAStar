using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private Vector2 targetCoordinates;

    private GameObject gameObjectToControl;

    private bool isObjectMovie = false;

    private void Awake()
    {
        gameObjectToControl = this.gameObject;
    }

    private List<Vector2> FindPath(Vector2 _start, Vector2 _target)
    {
        Debug.Log("Finding path from " + _start + " to " + _target);
        List<Vector2> path = new List<Vector2>();

        PriorityQueue<Vector2> frontier = new PriorityQueue<Vector2>();
        frontier.Enqueue(_start, 0);

        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        cameFrom[_start] = _start;

        Dictionary<Vector2, float> costSoFar = new Dictionary<Vector2, float>();
        costSoFar[_start] = 0;

        while (frontier.Count > 0)
        {
            Vector2 current = frontier.Dequeue();

            if (current.Equals(_target))
            {
                break;
            }

            foreach (Vector2 next in GetNeighbors(current))
            {
                if (GridGenerator.Instance.gridArray[(int)next.x, (int)next.y].IsObstacle)
                {
                    continue;
                }
                float newCost = costSoFar[current] + GetCost(current, next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + GetHeuristic(next, _target);
                    frontier.Enqueue(next, (int)priority);
                    cameFrom[next] = current;
                }
            }
        }

        Vector2 temp = _target;
        if (!cameFrom.ContainsKey(temp))
        {
            Debug.Log("No path found to target: " + _target);
            return path;
        }
        while (!temp.Equals(_start))
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }
        path.Reverse();

        Debug.Log("Path found: " + path[0].x + ", " + path[0].y);
        return path;
    }

    private float GetCost(Vector2 _from, Vector2 _to)
    {
        if (GridGenerator.Instance.gridArray[(int)_to.x, (int)_to.y].IsObstacle)
        {
            return Mathf.Infinity;
        }
        else
        {
            return Vector2.Distance(_from, _to);
        }
    }

    private List<Vector2> GetNeighbors(Vector2 _current)
    {
        List<Vector2> neighbors = new List<Vector2>();

        int x = (int)_current.x;
        int y = (int)_current.y;
        if (x > 0) neighbors.Add(GridGenerator.Instance.gridArray[x - 1, y].Coordinates);
        if (x < GridGenerator.Instance.gridWidth - 1) neighbors.Add(GridGenerator.Instance.gridArray[x + 1, y].Coordinates);
        if (y > 0) neighbors.Add(GridGenerator.Instance.gridArray[x, y - 1].Coordinates);
        if (y < GridGenerator.Instance.gridHeight - 1) neighbors.Add(GridGenerator.Instance.gridArray[x, y + 1].Coordinates);
        return neighbors;
    }

    private float GetHeuristic(Vector2 _next, Vector2 _target)
    {
        return Mathf.Abs(_next.x - _target.x) + Mathf.Abs(_next.y - _target.y);
    }

    public void SetTargetCoordinates(Vector2 _coordinates)
    {
        Debug.Log("Setting target coordinates to " + _coordinates);
        targetCoordinates = _coordinates;
        List<Vector2> path = FindPath((Vector2)gameObjectToControl.transform.position, targetCoordinates);
        //Debug.Log("Path from player to target: " + path[0].x + ", " + path[0].y);
        StartCoroutine(MovePlayerAlongPath(path));
    }

    private IEnumerator MovePlayerAlongPath(List<Vector2> _path)
    {
        if (isObjectMovie)
        {
            Debug.Log("Player is already moving. New movement command ignored.");
            yield break;
        }

        isObjectMovie = true;
        //Debug.Log("Moving player along path: " + _path[0].x + ", " + _path[0].y);
        foreach (Vector2 coordinates in _path)
        {
            Debug.Log("Moving player to " + coordinates);
            gameObjectToControl.transform.position = new Vector3(coordinates.x, coordinates.y, -0.5f);
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Player movement along path completed.");
        isObjectMovie = false;
    }
}