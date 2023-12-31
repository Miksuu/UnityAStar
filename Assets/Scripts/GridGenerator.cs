using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    // Singleton instance of the GridGenerator class
    public static GridGenerator Instance { get; private set; }

    // Grid dimensions
    public int gridWidth;
    public int gridHeight;

    // Percentage of grid cells that will be obstacles
    public int obstaclePercentage;

    // Prefab for grid cells
    public GameObject gridPrefab;

    // 2D array representing the grid
    public Grid[,] gridArray;

    // Material for obstacle cells
    private Material obstacleMaterial;

    // Materials for different types of cells
    public Material targetMaterial;
    public Material defaultMaterial;
    public Material calculatedMaterial;
    public Material pathMaterial;

    // Prefab for the player
    [SerializeField] public GameObject playerPrefab;

    // Reference to the player game object
    private GameObject playerGameobject { get; set; }

    // Awake is called when the script instance is being loaded
    // Ensures that only one instance of GridGenerator exists
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

    // Start is called before the first frame update
    // Initializes materials and generates the grid
    private void Start()
    {
        obstacleMaterial = Resources.Load<Material>("Materials/ObstacleMat");
        targetMaterial = Resources.Load<Material>("Materials/TargetMat");
        defaultMaterial = Resources.Load<Material>("Materials/DefaultMat");
        calculatedMaterial = Resources.Load<Material>("Materials/CalculatedMat");
        pathMaterial = Resources.Load<Material>("Materials/PathMat");

        Debug.Log("Obstacle material: " + obstacleMaterial);
        GenerateGrid();
        playerGameobject = Spawner.Instance.SpawnPlayer();
    }

    // Generates the grid by instantiating grid cells and setting their properties
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