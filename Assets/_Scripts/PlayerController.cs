using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using static UnityEngine.Camera;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    PlayerControl _inputs;
    Vector2 _move;

    [SerializeField] float _speed;

    [Header("Character Controller")]
    [SerializeField] CharacterController _controller;

    [Header("Movement")]
    [SerializeField] float _gravity = -30.0f;
    [SerializeField] float _jumpHeight = 3.0f;
    [SerializeField] Vector3 _velocity;

    [Header("Ground Detection")]
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _groudnRadius = 0.5f;
    [SerializeField] LayerMask _groundMaks;
    [SerializeField] bool _isGrounded;

    [Header("Virtual Camera")]
    [SerializeField] CinemachineVirtualCamera _vcam;

    [Header("Main Camera")]
    [SerializeField] Camera _mcam;

    void Awake()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
        _mcam = GetComponent<Camera>();
        _controller = GetComponent<CharacterController>();
        _inputs = new PlayerControl();
        _inputs.Player.Move.performed += context => _move = context.ReadValue<Vector2>();
        _inputs.Player.Move.canceled += ctx => _move = Vector2.zero;
        _inputs.Player.Jump.performed += ctx => Jump();
    }

    void OnEnable()
    {
        _inputs.Enable();
    }

    void OnDisable() => _inputs.Disable();

    void FixedUpdate()
    {
       _isGrounded = Physics.CheckSphere(_groundCheck.position, _groudnRadius, _groundMaks);
       if (_isGrounded && _velocity.y < 0.0f)
        {
            _velocity.y = -2.0f;
        }
        Vector3 movement = new Vector3(_move.x, 0.0f, _move.y) * _speed * Time.fixedDeltaTime;
        Vector3 cameraRelativeMovement = ConvertToCameraSpace(movement);
        _controller.Move(cameraRelativeMovement);
        _velocity.y += _gravity * Time.fixedDeltaTime;
        _controller.Move(_velocity * Time.fixedDeltaTime);
        //Debug.Log(_vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value);
    }

    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCameraSpace.y = currentYValue;
        return vectorRotatedToCameraSpace;
    }

   void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheck.position, _groudnRadius);
    }
    
    void Jump()
    {
       if (_isGrounded)
       {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
        }
    }

    void DebugMessage(InputAction.CallbackContext context)
    {
        Debug.Log($"Move Perfomed {context.ReadValue<Vector2>().x}, {context.ReadValue<Vector2>().y}");
    }
}