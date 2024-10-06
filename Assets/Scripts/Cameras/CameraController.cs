using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public float cameraMoveSpeed = 2f; // Adjust the speed at which the camera moves
    private Camera mainCamera;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Function to move the camera to the next tile
    public void MoveCameraToNextTile(Vector3 targetPosition, float delayTime)
    {
        StartCoroutine(SmoothMoveCamera(targetPosition, delayTime));
    }

    private IEnumerator SmoothMoveCamera(Vector3 targetPosition, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Cache the initial position of the camera
        Vector3 initialPosition = transform.position;
        
        float elapsedTime = 0;
        float journeyTime = Vector3.Distance(initialPosition, targetPosition) / cameraMoveSpeed;

        while (elapsedTime < journeyTime)
        {
            elapsedTime += Time.deltaTime;
            
            // Calculate the t parameter for SmoothStep
            float t = Mathf.Clamp01(elapsedTime / journeyTime);

            // Use SmoothStep for ease-in-out effect
            transform.position = Vector3.Lerp(initialPosition, targetPosition, Mathf.SmoothStep(0f, 1f, t));
            
            yield return null;
        }

        // Ensure the final position is exactly the target
        transform.position = targetPosition;
    }
}
