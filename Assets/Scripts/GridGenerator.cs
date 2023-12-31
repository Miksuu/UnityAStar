using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public static GridGenerator Instance { get; private set; }

    public int gridWidth;
    public int gridHeight;
    public int obstaclePercentage;
    public GameObject gridPrefab;
    public Grid[,] gridArray;
    private Material obstacleMaterial;
    public Material targetMaterial;
    public Material defaultMaterial;
    public Material calculatedMaterial;

    [SerializeField] public GameObject playerPrefab;
    private GameObject playerGameobject { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        obstacleMaterial = Resources.Load<Material>("Materials/ObstacleMat");
        targetMaterial = Resources.Load<Material>("Materials/TargetMat");
        defaultMaterial = Resources.Load<Material>("Materials/DefaultMat");
        calculatedMaterial = Resources.Load<Material>("Materials/CalculatedMat");

        Debug.Log("Obstacle material: " + obstacleMaterial);
        GenerateGrid();
        playerGameobject = Spawner.Instance.SpawnPlayer();
    }

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
}