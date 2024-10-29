/*
 * A very simple FPS character controller
 */

using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("Units are degrees per full viewport scroll")]
    private float _lookSensivity = 10.0f;
    [SerializeField, Tooltip("In meters per second")]
    private float _walkSpeed = 5.75f;
    [SerializeField, Tooltip("In meters per squared seconds")]
    private float _walkAcceleration = 50f;
    [SerializeField, Tooltip("In meters")]
    private float _jumpHeight = 1.25f;
    [SerializeField]
    private Vector3 _gravity = new Vector3(0.0f, -9.81f, 0.0f);
    [SerializeField, Tooltip("Assign child main camera")]
    private Camera _camera;
    [SerializeField]
    private BobController _bobController;

    private CharacterController _characterController;
    private Vector3 _velocityPlanar;
    private Vector3 _velocityGravity;
    private float _aizmuth;
    private float _altitude;

    // Grab attached controller, lock mouse, and reset default azimuth/altitude
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _aizmuth = transform.eulerAngles.y;
        _altitude = 0.0f;
    }
    private void Update()
    {
        // Grab desired movement
        var movementDesired = Vector3.zero;

        // Check desired direction
        // Notice we are using local right nad local forward (*transform* ones, not *Vector3* ones!)
        movementDesired += Input.GetAxis("Horizontal") * transform.right;
        movementDesired += Input.GetAxis("Vertical") * transform.forward;

        // We are not moving faster then 100% of our walk speed
        // In case we are moving diagonally
        if (movementDesired.sqrMagnitude > 1.0f)
        {
            movementDesired.Normalize();
        }

        _bobController.WalkingSpeed = movementDesired.magnitude;

        // Calculate final planar velocity and move actual velocity toward it
        _velocityPlanar = Vector3.MoveTowards(_velocityPlanar, movementDesired * _walkSpeed, _walkAcceleration * Time.deltaTime);

        // If we are grounded, feel free to jump
        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            _velocityGravity = new Vector3(0.0f, Mathf.Sqrt(_jumpHeight * _gravity.y * -2.0f), 0.0f);
            _bobController.IsJumping = true;
        }
        // If not, we should start to fall
        if (!_characterController.isGrounded)
        {
            _velocityGravity += _gravity * Time.deltaTime;
            if (_velocityGravity.y <= 0.0f)
            {
                _bobController.IsJumping = false;
            }
        }
        // If we are grounded and not jumping, the gravity should not accumulate infinitely
        // But it also shouldn't be zero-ed, as you may be going down-slope
        else if (_velocityGravity.y <= 0.0f)
        {
            _velocityGravity = _gravity * 0.5f;
            _bobController.IsJumping = false;
        }

        // The famous double-move of Unity character controller
        // The order is very important, as second move will populate isGrounded flag
        _characterController.Move(_velocityPlanar * Time.deltaTime);
        _characterController.Move(_velocityGravity * Time.deltaTime);

        _bobController.IsGrounded = _characterController.isGrounded;

        // Look around with mouse
        _aizmuth += Input.GetAxis("Mouse X") * _lookSensivity;
        _altitude += Input.GetAxis("Mouse Y") * _lookSensivity;

        // Do not allow backroll-flip
        _altitude = Mathf.Clamp(_altitude, -90.0f, 90.0f);

        // Rotate the main character for left/right
        transform.eulerAngles = new Vector3(0.0f, _aizmuth, 0.0f);
        // Rotate the camera only for up/bottom
        _camera.transform.localEulerAngles = new Vector3(-_altitude, 0.0f, 0.0f);
    }
}
