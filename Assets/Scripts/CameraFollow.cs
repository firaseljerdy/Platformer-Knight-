using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = new Vector2(0f, 2.5f);
    [SerializeField, Min(0f)] private float smoothTime = 0.15f;
    [SerializeField] private float minimumY = 0f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 goal = new Vector3(target.position.x + offset.x,
            Mathf.Max(minimumY, target.position.y + offset.y),
            transform.position.z);

        transform.position = Vector3.SmoothDamp
            (transform.position, goal, ref velocity, smoothTime);
    }


}
