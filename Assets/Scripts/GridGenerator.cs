using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int obstaclePercentage;

    [SerializeField] private GameObject gridPrefab;
    private Grid[,] gridArray;
    private Material obstacleMaterial;

    private void Start()
    {
        obstacleMaterial = Resources.Load<Material>("Materials/ObstacleMat");
        Debug.Log("Obstacle material: " + obstacleMaterial);
        GenerateGrid();
        SpawnPlayer();
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
                GameObject instance = Instantiate(gridPrefab, new Vector3(x, y, 0f), Quaternion.identity);
                gridArray[x, y] = instance.GetComponent<Grid>();
                if (Random.Range(0, 100) < obstaclePercentage)
                {
                    instance.GetComponent<MeshRenderer>().materials[0] = obstacleMaterial;
                    gridArray[x, y].IsObstacle = true;
                }
                gridArray[x, y].Coordinates = new Vector2(x, y);
            }
        }
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