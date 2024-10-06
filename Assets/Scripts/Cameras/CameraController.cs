using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Transform cameraTransform;  // Reference to the camera's transform
    public float moveSpeed = 100f;       // Speed at which the camera moves

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Persist between scenes
        }
        else
        {
            Destroy(gameObject);  // Ensure only one instance exists
        }

        cameraTransform = Camera.main.transform;  // Set to the main camera
    }

    // Move the camera smoothly to a target position over time
    public void MoveCamera(Vector3 targetPosition, System.Action onComplete = null)
    {
        StartCoroutine(MoveCameraCoroutine(targetPosition, onComplete));
    }

    private IEnumerator MoveCameraCoroutine(Vector3 targetPosition, System.Action onComplete)
    {
        while (Vector3.Distance(cameraTransform.position, targetPosition) > 0.1f)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        cameraTransform.position = targetPosition;  // Snap to the exact final position

        // Invoke the callback once the camera reaches the target
        onComplete?.Invoke();
    }
}
