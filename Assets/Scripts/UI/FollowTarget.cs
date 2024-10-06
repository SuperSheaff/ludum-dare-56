using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target; // The target to follow

    private void Update()
    {
        if (target != null)
        {
            // Follow the target's position
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
