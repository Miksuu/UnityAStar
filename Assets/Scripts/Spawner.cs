using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }

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
