using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float heightOffset = 10;
    [SerializeField]
    private float lookOffset = 3;
    [SerializeField]
    private float forwardOffset = 3;
    [SerializeField]
    private float smoothness = 8f;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private CameraBounds cameraBounds;

    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        var nextPosition = target.position + Vector3.up * heightOffset + target.forward * lookOffset + new Vector3(0, 0, 1) * forwardOffset;
        if (cameraBounds != null)
        {
            nextPosition = BoundCameraPosition(nextPosition);
        }
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, nextPosition, Time.deltaTime * smoothness);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private Vector3 BoundCameraPosition(Vector3 nextPosition)
    {
        if (nextPosition.z > cameraBounds.Top.position.z)
        {
            nextPosition.z = cameraBounds.Top.position.z;
        }

        if (nextPosition.z < cameraBounds.Bottom.position.z)
        {
            nextPosition.z = cameraBounds.Bottom.position.z;
        }

        if (nextPosition.x > cameraBounds.Right.position.x)
        {
            nextPosition.x = cameraBounds.Right.position.x;
        }

        if (nextPosition.x < cameraBounds.Left.position.x)
        {
            nextPosition.x = cameraBounds.Left.position.x;
        }

        return nextPosition;
    }
}
