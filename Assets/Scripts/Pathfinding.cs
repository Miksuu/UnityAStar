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

    // Awake is called when the script instance is being loaded.
    // It initializes the gameObjectToControl with the GameObject this script is attached to.
    private void Awake()
    {
        gameObjectToControl = this.gameObject;
    }

    // FindPath method is used to find the shortest path from the start grid to the target grid.
    // It uses the A* algorithm for pathfinding.
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

    // GetCost method is used to calculate the cost of moving from one grid to another.
    // If the destination grid is an obstacle, the cost is infinity.
    // Otherwise, the cost is the distance between the two grids.
    private float GetCost(Grid _fromGrid, Grid _toGrid)
    {
        return _toGrid.IsObstacle ? Mathf.Infinity : Vector2.Distance(_fromGrid.Coordinates, _toGrid.Coordinates);
    }

    // This method is used to get all the neighboring grids of the current grid.
    // It checks the boundaries of the grid to avoid out of range errors.
    // It returns a list of all the neighboring grids including diagonals.
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

    // This method calculates the heuristic cost for a grid node in the pathfinding algorithm.
    // It uses the Manhattan distance (sum of absolute differences in x and y coordinates) as the heuristic.
    private float GetHeuristic(Grid _nextGrid, Grid _targetGrid)
    {
        return Mathf.Abs(_nextGrid.Coordinates.x - _targetGrid.Coordinates.x) + Mathf.Abs(_nextGrid.Coordinates.y - _targetGrid.Coordinates.y);
    }

    // This method sets the target coordinates for the pathfinding algorithm.
    // It checks if the object is already moving or if the target grid is an obstacle before setting the target coordinates.
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

    // This method sets the material of the path grids to the path material.
    private void SetPathMaterial(List<Grid> _path)
    {
        foreach (Grid grid in _path)
        {
            var meshRenderer = grid.GetComponent<MeshRenderer>();
            meshRenderer.material = GridGenerator.Instance.pathMaterial;
        }
    }

    // This method moves the object along the path.
    // It changes the object's position to each grid in the path with a delay between each movement.
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

    // This method resets the cost of all updated grids to 0 and clears the list of updated grids.
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

