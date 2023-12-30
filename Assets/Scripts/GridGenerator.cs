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
    private GameObject playerGameobject;

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
                    var meshRenderer = instance.GetComponent<MeshRenderer>();
                    meshRenderer.material = obstacleMaterial;
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
        playerGameobject = Instantiate(playerPrefab, new Vector3(x, y, -0.5f), Quaternion.identity);

        Debug.Log("Player spawned at: " + x + ", " + y);
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.W))
    //     {
    //         MovePlayer(Vector2.up);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         MovePlayer(Vector2.down);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         MovePlayer(Vector2.left);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.D))
    //     {
    //         MovePlayer(Vector2.right);
    //     }
    // }

    private void MovePlayer(Vector2 direction)
    {
        Debug.Log("Player: " + playerGameobject);
        Debug.Log("Player Transform: " + playerGameobject.transform);
        Debug.Log("Player Position: " + playerGameobject.transform.position);
        Debug.Log("Direction: " + direction);

        Vector2 newCoordinates = playerGameobject.transform.position + new Vector3(direction.x, direction.y, 0);
        Debug.Log("New Coordinates: " + newCoordinates);

        if (newCoordinates.x >= 0 && newCoordinates.x < gridWidth && newCoordinates.y >= 0 && newCoordinates.y < gridHeight)
        {
            if (!gridArray[(int)newCoordinates.x, (int)newCoordinates.y].IsObstacle)
            {
                playerGameobject.transform.position = new Vector3(newCoordinates.x, newCoordinates.y, -0.5f);
            }
        }
    }
}