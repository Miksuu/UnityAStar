using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int obstaclePercentage;

    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject obstaclePrefab;

    private Grid[,] gridArray;

    private void Start()
    {
        GenerateGrid();
    }

    [SerializeField] private GameObject playerPrefab;
    private Player player;

    private void GenerateGrid()
    {
        gridArray = new Grid[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject toInstantiate = Random.Range(0, 100) < obstaclePercentage ? obstaclePrefab : gridPrefab;
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                gridArray[x, y] = instance.GetComponent<Grid>();
                gridArray[x, y].IsObstacle = toInstantiate == obstaclePrefab;
                gridArray[x, y].Coordinates = new Vector2(x, y);
            }
        }
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        int x = Random.Range(0, gridWidth);
        int y = Random.Range(0, gridHeight);
        while (gridArray[x, y].IsObstacle)
        {
            x = Random.Range(0, gridWidth);
            y = Random.Range(0, gridHeight);
        }
        GameObject playerInstance = Instantiate(playerPrefab, new Vector3(x, y, -0.5f), Quaternion.identity);
        player = playerInstance.GetComponent<Player>();
    }
}