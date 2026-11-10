using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField, Range(0f, 1f)] private float parallaxX = 0.5f;
    [SerializeField, Range(0f, 1f)] private float parallaxY = 0.5f;

    [SerializeField] private bool loopHorizontally = true;
    [SerializeField] private float autoScrollSpeed = 0f;

    private Vector3 lastCameraPosition;
    private float tileWidth;

    private void Start()
    {

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraTransform == null)
        {
            Debug.LogError("No camera available on the scene.");
            enabled = false;
            return;
        }

        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        tileWidth = sprite == null ? 0f : sprite.bounds.size.x * transform.lossyScale.x;
    }

    private void LateUpdate()
    {
        Vector3 camerDelta = cameraTransform.position - lastCameraPosition;
        lastCameraPosition = cameraTransform.position;

        transform.position += new Vector3(
            camerDelta.x * parallaxX + autoScrollSpeed * Time.deltaTime,
            camerDelta.y * parallaxY,
            0f);

        if (!loopHorizontally || tileWidth <= 0f) return;
    }

}
