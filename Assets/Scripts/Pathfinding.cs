using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private Grid targetGrid;
    private GameObject gameObjectToControl;
    private bool isObjectMoving = false;
    private List<Grid> updatedGrids = new List<Grid>();
    private float movementDelayBetweenGrids = 0.1f;

    private void Awake()
    {
        gameObjectToControl = this.gameObject;
    }

    private List<Grid> FindPath(Grid _startGrid, Grid _targetGrid)
    {
        Debug.Log($"Finding path from {_startGrid.Coordinates} to {_targetGrid.Coordinates}");
        List<Grid> path = new List<Grid>();
        PriorityQueue<Grid> frontier = new PriorityQueue<Grid>();
        frontier.Enqueue(_startGrid, 0);

        Dictionary<Grid, Grid> cameFrom = new Dictionary<Grid, Grid> { { _startGrid, _startGrid } };
        Dictionary<Grid, float> costSoFar = new Dictionary<Grid, float> { { _startGrid, 0 } };

        while (frontier.Count > 0)
        {
            Grid current = frontier.Dequeue();

            if (current.Equals(_targetGrid))
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
                    float priority = newCost + GetHeuristic(next, _targetGrid);
                    frontier.Enqueue(next, (int)priority);
                    cameFrom[next] = current;
                }
            }
        }

        Grid temp = _targetGrid;
        if (!cameFrom.ContainsKey(temp))
        {
            Debug.Log($"No path found to target: {_targetGrid.Coordinates}");
            return path;
        }
        while (!temp.Equals(_startGrid))
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }
        path.Reverse();

        return path;
    }

    private float GetCost(Grid _fromGrid, Grid _toGrid)
    {
        return _toGrid.IsObstacle ? Mathf.Infinity : Vector2.Distance(_fromGrid.Coordinates, _toGrid.Coordinates);
    }

    private List<Grid> GetNeighbors(Grid _currentGrid)
    {
        List<Grid> neighbors = new List<Grid>();
        int x = (int)_currentGrid.Coordinates.x;
        int y = (int)_currentGrid.Coordinates.y;
        GridGenerator gridGenerator = GridGenerator.Instance;

        if (x > 0) neighbors.Add(gridGenerator.gridArray[x - 1, y]);
        if (x < gridGenerator.gridWidth - 1) neighbors.Add(gridGenerator.gridArray[x + 1, y]);
        if (y > 0) neighbors.Add(gridGenerator.gridArray[x, y - 1]);
        if (y < gridGenerator.gridHeight - 1) neighbors.Add(gridGenerator.gridArray[x, y + 1]);
        if (x > 0 && y > 0) neighbors.Add(gridGenerator.gridArray[x - 1, y - 1]);
        if (x < gridGenerator.gridWidth - 1 && y < gridGenerator.gridHeight - 1) neighbors.Add(gridGenerator.gridArray[x + 1, y + 1]);
        if (x > 0 && y < gridGenerator.gridHeight - 1) neighbors.Add(gridGenerator.gridArray[x - 1, y + 1]);
        if (x < gridGenerator.gridWidth - 1 && y > 0) neighbors.Add(gridGenerator.gridArray[x + 1, y - 1]);
        return neighbors;
    }

    private float GetHeuristic(Grid _nextGrid, Grid _targetGrid)
    {
        return Mathf.Abs(_nextGrid.Coordinates.x - _targetGrid.Coordinates.x) + Mathf.Abs(_nextGrid.Coordinates.y - _targetGrid.Coordinates.y);
    }

    public void SetTargetCoordinates(Grid _grid)
    {
        Debug.Log($"Setting target coordinates to {_grid.Coordinates}");

        if (isObjectMoving)
        {
            Debug.Log("Player is already moving. New movement command ignored.");
            return;
        }

        if (_grid.IsObstacle)
        {
            Debug.Log($"Obstacle found at {_grid.Coordinates}. Pathfinding aborted.");
            return;
        }

        targetGrid = _grid;
        GridGenerator gridGenerator = GridGenerator.Instance;
        List<Grid> path = FindPath(gridGenerator.gridArray[(int)gameObjectToControl.transform.position.x, (int)gameObjectToControl.transform.position.y], targetGrid);
        
        SetPathMaterial(path);

        var meshRenderer = _grid.GetComponent<MeshRenderer>();
        meshRenderer.material = gridGenerator.targetMaterial;

        StartCoroutine(MoveObjectAlongPath(path));
    }

    private void SetPathMaterial(List<Grid> _path)
    {
        foreach (Grid grid in _path)
        {
            var meshRenderer = grid.GetComponent<MeshRenderer>();
            meshRenderer.material = GridGenerator.Instance.pathMaterial;
        }
    }

    private IEnumerator MoveObjectAlongPath(List<Grid> _path)
    {
        isObjectMoving = true;
        foreach (Grid grid in _path)
        {
            Debug.Log($"Moving player to {grid.Coordinates}");
            gameObjectToControl.transform.position = new Vector3(grid.Coordinates.x, grid.Coordinates.y, -0.5f);
            yield return new WaitForSeconds(movementDelayBetweenGrids);
        }

        var meshRenderer = targetGrid.GetComponent<MeshRenderer>();
        meshRenderer.material = GridGenerator.Instance.defaultMaterial;

        ResetGridsCost();

        Debug.Log("Player movement along path completed.");
        isObjectMoving = false;
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

