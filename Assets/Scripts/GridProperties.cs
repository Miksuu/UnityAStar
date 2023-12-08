using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridProperties : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int obstaclePercentage;

    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject obstaclePrefab;

    private Grid[,] gridArray;

    private void Start()
    {
        gridArray = new Grid[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject toInstantiate = Random.Range(0, 100) < obstaclePercentage ? obstaclePrefab : gridPrefab;
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                gridArray[x, y] = instance.GetComponent<Grid>();
                //gridArray[x, y].IsObstacle = toInstantiate == obstaclePrefab;
                //gridArray[x, y].Coordinates = new Vector2(x, y);
            }
        }
    }
}
