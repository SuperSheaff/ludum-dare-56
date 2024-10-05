using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    public GameObject marker; // The UI marker above the target

    // Show the marker
    public void ShowMarker()
    {
        marker.SetActive(true);
    }

    // Hide the marker
    public void HideMarker()
    {
        marker.SetActive(false);
    }

    // Handle click on the target
    private void OnMouseDown()
    {
        // Notify the game controller that this target was chosen
        GameController.instance.OnTargetChosen(this.gameObject);
    }
}