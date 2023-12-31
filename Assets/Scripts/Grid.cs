using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grid : MonoBehaviour
{
    public bool IsObstacle { get; set; }
    public Vector2 Coordinates { get; set; }
    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnMouseDown()
    {
        Debug.Log("Grid at " + Coordinates + " clicked.");

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.GetComponent<Pathfinding>().SetTargetCoordinates(this);
    }

    public void UpdateCostText(int _cost)
    {
        if (textComponent != null)
        {
            textComponent.text = _cost.ToString();
        }
        else
        {
            Debug.LogError("TextMeshPro component not found!");
        }
    }
}