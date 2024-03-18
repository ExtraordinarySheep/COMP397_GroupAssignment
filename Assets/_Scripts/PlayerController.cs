using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using static UnityEngine.Camera;
using Slider = UnityEngine.UI.Slider;
using UnityEngine.UI; 
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    PlayerControl _inputs;
    Vector2 _move;
    public Vector2 mouseDelta;

    [SerializeField] float _speed;

    [Header("Mobile Controls")]
    [SerializeField] Joystick _joystick;
    [SerializeField] Button _turnCamLeftBtn;
    [SerializeField] Button _turnCamRightBtn;
    [SerializeField] int _rotationValue;
    [SerializeField] Button _jumpBtn;


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
    [SerializeField] CinemachineFreeLook _vcam;



    [Header("Main Camera")]
    [SerializeField] Camera _mcam;

    [Header("Health")]
    [SerializeField] public GameObject hpBarSliderForPlayer;
    private Slider hpSlider;
    int _health = 100;
    AudioManager am;
    AudioSource _audioSource;

    [Header("Respawn Transform")]
    [SerializeField] Transform _respawn; 

    void Awake()
    {

        _mcam = GetComponent<Camera>();
        //_vcam = GetComponent<CinemachineFreeLook>(); 
        _controller = GetComponent<CharacterController>();
        _inputs = new PlayerControl();
        _inputs.Player.Move.performed += context => _move = context.ReadValue<Vector2>();
        _inputs.Player.Move.canceled += ctx => _move = Vector2.zero;
        _inputs.Player.Jump.performed += ctx => Jump();
        _audioSource = GameObject.Find("AudioController").GetComponent<AudioSource>();
        _turnCamLeftBtn.onClick.AddListener(() => RotateCamera(-_rotationValue));
        _turnCamRightBtn.onClick.AddListener(() => RotateCamera(_rotationValue));
        _jumpBtn.onClick.AddListener(Jump);
            
        //Hide Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (SaveGameManager.Instance().playerPosition != Vector3.zero && SaveGameManager.Instance().saveLoaded == true)
        {
            _controller.enabled = false;
            gameObject.transform.position = SaveGameManager.Instance().playerPosition;
            _controller.enabled = true;
        }
    }
    public void Update()
    {
        HealthBarForPlayer();
        if (_health <= 0)
        {
            _audioSource.Stop();
            Die();
        }
        mouseDelta = Mouse.current.delta.ReadValue();

        //prevent joystick from moving screen 
        bool isJoystickActive = _joystick.Direction.magnitude > 0.5f;
        if (isJoystickActive)
        {
            //_vcam.m_XAxis.Value = _currentRotationValue;
            _vcam.m_YAxis.Value = 0;
        }

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

        Vector2 joystickMove = _joystick.Direction;
        bool isJoystickActive = joystickMove.magnitude > 0.1f; 

        // Determine the final movement input based on input state (allows for input system to still work) 
        Vector2 finalMoveInput = isJoystickActive ? joystickMove : (_inputs.Player.Move.ReadValue<Vector2>());
        Vector3 movement = new Vector3(finalMoveInput.x, 0.0f, finalMoveInput.y) * _speed * Time.fixedDeltaTime;
        //Vector3 movement = new Vector3(_move.x, 0.0f, _move.y) * _speed * Time.fixedDeltaTime;

        Vector3 cameraRelativeMovement = ConvertToCameraSpace(movement);
        _controller.Move(cameraRelativeMovement);
        _velocity.y += _gravity * Time.fixedDeltaTime;
        _controller.Move(_velocity * Time.fixedDeltaTime);
        //_move = _joystick.Direction;

        //Debug.Log(_isGrounded);
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

    void RotateCamera(int angle)
    {
        if (_vcam != null)
        {
            _vcam.m_XAxis.Value += angle;

        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera is not assigned.");
        }

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
    private void Die()
    {
        SceneManager.LoadScene("GameOverScene");

    }
    public void Damage(int amount)
    {
        _health -= amount;

        // Update HP bar value  
        if (hpBarSliderForPlayer != null)
        {
            Debug.Log("Took " + amount + " damage From Player Controller");
            // Ensure that the health value stays within the range [0, 100]
            _health = Mathf.Clamp(_health, 0, 100);

            // Calculate the normalized value for the slider based on the health
            float normalizedHealth = (float)_health / 100f;

            // Update the slider value
            hpSlider.value = normalizedHealth;
        }
    }
    void HealthBarForPlayer()
    {

        hpSlider = hpBarSliderForPlayer.GetComponent<Slider>();

    }
    public void Respawn()
    {
        _velocity = Vector3.zero;
    }
    void DebugMessage(InputAction.CallbackContext context)
    {
        Debug.Log($"Move Perfomed {context.ReadValue<Vector2>().x}, {context.ReadValue<Vector2>().y}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Colliding with {other.tag}");
        if (other.CompareTag("DeathZone"))
        {
            _controller.enabled = false;
            transform.position = _respawn.position;
            Damage(25); 
            _controller.enabled = true;
        }
        else if (other.CompareTag("Enemy"))
        {
            Damage(5);
        }
        else if (other.CompareTag("Goal"))
        {
            LoadScene.Instance().MainMenuButton();
        }
    }


}