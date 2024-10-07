using UnityEngine;

public class CameraMouseFollow : MonoBehaviour
{
    public float sensitivity = 0.1f; // Sensitivity of the movement
    public float maxOffset = 1.0f; // Max amount the camera can move from its center position
    private Vector3 originalPosition; // Store the original position of the camera
    private Vector2 screenCenter; // Screen center in pixels

    private void Start()
    {
        // Store the initial position of the camera as the "center" point
        originalPosition = transform.position;

        // Set the screen center in pixel coordinates
        screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    private void Update()
    {
        ParallaxEffect();
    }

    private void ParallaxEffect()
    {
        // Get the mouse position relative to the screen center
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseOffset = (mousePosition - screenCenter) / screenCenter;

        // Calculate the new position based on the mouse offset and sensitivity
        float xOffset = Mathf.Clamp(mouseOffset.x * sensitivity, -maxOffset, maxOffset);
        float yOffset = Mathf.Clamp(mouseOffset.y * sensitivity, -maxOffset, maxOffset);

        // Apply the offset to the original camera position
        Vector3 targetPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);

        // Move the camera to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2f);
    }
}
