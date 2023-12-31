using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grid : MonoBehaviour
{
    public bool IsObstacle { get; set; }
    public Vector2 Coordinates { get; set; }
    private TextMeshProUGUI textComponent;

    // This method is called when the script instance is being loaded.
    // It initializes the textComponent with the TextMeshProUGUI component of the GameObject.
    void Start()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
    }

    // This method is called when the user presses the mouse button while over the Collider.
    // It logs the coordinates of the clicked grid and sets it as the target coordinates for the player's pathfinding.
    private void OnMouseDown()
    {
        Debug.Log("Grid at " + Coordinates + " clicked.");

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.GetComponent<Pathfinding>().SetTargetCoordinates(this);
    }

    // This method updates the cost text displayed on the grid.
    // If the TextMeshProUGUI component is not found, it logs an error.
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