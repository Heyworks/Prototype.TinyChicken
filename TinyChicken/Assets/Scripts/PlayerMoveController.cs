// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class PlayerMoveController : MonoBehaviour
{
    // Objects to drag in
    public FreeMovementMotor motor;
    public Transform character;

    // Private memeber data
    private Camera mainCamera;
    private Joystick joystickLeft;
    private Transform mainCameraTransform;

    private Quaternion screenMovementSpace;
    private Vector3 screenMovementForward;
    private Vector3 screenMovementRight;

    void IsLocalPlayer(bool val)
    {
        this.enabled = val;

    }

    void Awake()
    {
        motor.movementDirection = Vector2.zero;
        motor.facingDirection = Vector2.zero;

        // Set main camera
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera.transform;

        // Ensure we have character set
        // Default to using the transform this component is on
        if (!character)
            character = transform;
    }

    void Start()
    {
        joystickLeft = GameObject.FindGameObjectWithTag("LeftStick").GetComponent<Joystick>();

        // it's fine to calculate this on Start () as the camera is static in rotation
        screenMovementSpace = Quaternion.Euler(0, mainCameraTransform.eulerAngles.y, 0);
        screenMovementForward = screenMovementSpace * Vector3.forward;
        screenMovementRight = screenMovementSpace * Vector3.right;
    }

    void Update()
    {
        // HANDLE CHARACTER MOVEMENT DIRECTION
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
        if (joystickLeft != null)
        {
            Vector3 joystickDirection = joystickLeft.Value.x * screenMovementRight + joystickLeft.Value.y * screenMovementForward;
            if (joystickLeft.IsActivated)
            {
                motor.movementDirection = joystickDirection;
            }
            else
            {
                motor.movementDirection = Vector3.zero;
            }

            motor.facingDirection = joystickDirection.normalized;
        }
#else
        motor.movementDirection = Input.GetAxis("Horizontal") * screenMovementRight + Input.GetAxis("Vertical") * screenMovementForward;
        motor.facingDirection = Input.GetAxis("Horizontal") * screenMovementRight + Input.GetAxis("Vertical") * screenMovementForward;
#endif

        // Make sure the direction vector doesn't exceed a length of 1
        // so the character can't move faster diagonally than horizontally or vertically
        if (motor.movementDirection.sqrMagnitude > 1)
            motor.movementDirection.Normalize();


        // used to adjust the camera based on cursor or joystick position

        //if (joystickRight.IsCaptured)
        //{
        //    motor.facingDirection = (joystickRight.Value.x * screenMovementRight + joystickRight.Value.y * screenMovementForward).normalized;
        //}
        //else
        //{
        //    motor.facingDirection = (joystickLeft.Value.x * screenMovementRight + joystickLeft.Value.y * screenMovementForward).normalized;
        //}
    }

    public static Vector3 PlaneRayIntersection(Plane plane, Ray ray)
    {
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    public static Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera camera)
    {
        // Set up a ray corresponding to the screen position
        Ray ray = camera.ScreenPointToRay(screenPoint);

        // Find out where the ray intersects with the plane
        return PlaneRayIntersection(plane, ray);
    }
}