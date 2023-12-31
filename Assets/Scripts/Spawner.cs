using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Singleton instance of the Spawner class
    public static Spawner Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    // Used here to implement the Singleton pattern
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

    // Method to spawn a player at a random non-obstacle grid position
    // Returns the spawned GameObject
    public GameObject SpawnPlayer()
    {
        int x = Random.Range(0, GridGenerator.Instance.gridWidth);
        int y = Random.Range(0, GridGenerator.Instance.gridHeight);
        while (GridGenerator.Instance.gridArray[x, y].IsObstacle)
        {
            x = Random.Range(0, GridGenerator.Instance.gridWidth);
            y = Random.Range(0, GridGenerator.Instance.gridHeight);
        }
        GameObject spawnedPlayer = Instantiate(
            GridGenerator.Instance.playerPrefab, new Vector3(x, y, -0.5f), Quaternion.identity);

        Debug.Log("Player spawned at: " + x + ", " + y);
        return spawnedPlayer;
    }
}
