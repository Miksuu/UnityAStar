using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool IsObstacle { get; set; }
    public Vector2 Coordinates { get; set; }

    private void OnMouseDown()
    {
        Debug.Log("Grid at " + Coordinates + " clicked.");
    }
}
