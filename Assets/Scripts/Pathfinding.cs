using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private Grid targetGrid;

    private GameObject gameObjectToControl;

    private bool isObjectMovie = false;
    private List<Grid> updatedGrids = new List<Grid>();

    private void Awake()
    {
        gameObjectToControl = this.gameObject;
    }

    private List<Grid> FindPath(Grid _start, Grid _target)
    {
        Debug.Log("Finding path from " + _start.Coordinates + " to " + _target.Coordinates);
        List<Grid> path = new List<Grid>();

        PriorityQueue<Grid> frontier = new PriorityQueue<Grid>();
        frontier.Enqueue(_start, 0);

        Dictionary<Grid, Grid> cameFrom = new Dictionary<Grid, Grid>();
        cameFrom[_start] = _start;

        Dictionary<Grid, float> costSoFar = new Dictionary<Grid, float>();
        costSoFar[_start] = 0;

        while (frontier.Count > 0)
        {
            Grid current = frontier.Dequeue();

            if (current.Equals(_target))
            {
                break;
            }

            foreach (Grid next in GetNeighbors(current))
            {
                if (next.IsObstacle)
                {
                    continue;
                }
                float newCost = costSoFar[current] + GetCost(current, next);

                var meshRenderer = next.GetComponent<MeshRenderer>();
                meshRenderer.material = GridGenerator.Instance.calculatedMaterial;

                next.UpdateCostText((int)newCost);
                updatedGrids.Add(next);
                
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + GetHeuristic(next, _target);
                    frontier.Enqueue(next, (int)priority);
                    cameFrom[next] = current;
                }
            }
        }

        Grid temp = _target;
        if (!cameFrom.ContainsKey(temp))
        {
            Debug.Log("No path found to target: " + _target.Coordinates);
            return path;
        }
        while (!temp.Equals(_start))
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }
        path.Reverse();

        //Debug.Log("Path found: " + path[0].Coordinates.x + ", " + path[0].Coordinates.y);
        return path;
    }

    private float GetCost(Grid _from, Grid _to)
    {
        //Debug.Log("_from: " + _from.Coordinates.x + ", " + _from.Coordinates.y + " _to: " + _to.Coordinates.x + ", " + _to.Coordinates.y);

        if (_to.IsObstacle)
        {
            return Mathf.Infinity;
        }
        else
        {
            return Vector2.Distance(_from.Coordinates, _to.Coordinates);
        }
    }

    private List<Grid> GetNeighbors(Grid _current)
    {
        List<Grid> neighbors = new List<Grid>();

        int x = (int)_current.Coordinates.x;
        int y = (int)_current.Coordinates.y;
        if (x > 0) neighbors.Add(GridGenerator.Instance.gridArray[x - 1, y]);
        if (x < GridGenerator.Instance.gridWidth - 1) neighbors.Add(GridGenerator.Instance.gridArray[x + 1, y]);
        if (y > 0) neighbors.Add(GridGenerator.Instance.gridArray[x, y - 1]);
        if (y < GridGenerator.Instance.gridHeight - 1) neighbors.Add(GridGenerator.Instance.gridArray[x, y + 1]);
        return neighbors;
    }

    private float GetHeuristic(Grid _next, Grid _target)
    {
        return Mathf.Abs(_next.Coordinates.x - _target.Coordinates.x) + Mathf.Abs(_next.Coordinates.y - _target.Coordinates.y);
    }

    public void SetTargetCoordinates(Grid _grid)
    {
        Debug.Log("Setting target coordinates to " + _grid.Coordinates);

        targetGrid = _grid;

        List<Grid> path = FindPath(
            GridGenerator.Instance.gridArray[
                (int)gameObjectToControl.transform.position.x, (int)gameObjectToControl.transform.position.y], targetGrid);
        
        SetPathMaterial(path);

        var meshRenderer = _grid.GetComponent<MeshRenderer>();
        meshRenderer.material = GridGenerator.Instance.targetMaterial;

        StartCoroutine(MovePlayerAlongPath(path));
    }

    private void SetPathMaterial(List<Grid> _path)
    {
        foreach (Grid grid in _path)
        {
            var meshRenderer = grid.GetComponent<MeshRenderer>();
            meshRenderer.material = GridGenerator.Instance.pathMaterial;
        }
    }

    private IEnumerator MovePlayerAlongPath(List<Grid> _path)
    {
        if (isObjectMovie)
        {
            Debug.Log("Player is already moving. New movement command ignored.");
            yield break;
        }

        isObjectMovie = true;
        //Debug.Log("Moving player along path: " + _path[0].Coordinates.x + ", " + _path[0].Coordinates.y);
        foreach (Grid grid in _path)
        {
            Debug.Log("Moving player to " + grid.Coordinates);
            gameObjectToControl.transform.position = new Vector3(grid.Coordinates.x, grid.Coordinates.y, -0.5f);
            yield return new WaitForSeconds(0.1f);
        }

        var meshRenderer = targetGrid.GetComponent<MeshRenderer>();
        meshRenderer.material = GridGenerator.Instance.defaultMaterial;

        ResetGridsCost();

        Debug.Log("Player movement along path completed.");
        isObjectMovie = false;
    }

    private void ResetGridsCost()
    {
        foreach (Grid grid in updatedGrids)
        {
            var meshRenderer = grid.GetComponent<MeshRenderer>();
            meshRenderer.material = GridGenerator.Instance.defaultMaterial;

            grid.UpdateCostText(0);
        }
        updatedGrids.Clear();
    }
}
