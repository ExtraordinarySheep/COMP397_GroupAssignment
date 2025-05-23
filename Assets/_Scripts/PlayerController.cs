using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using static UnityEngine.Camera;
using Slider = UnityEngine.UI.Slider;
using UnityEngine.UI; 
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : Subject, IObserver
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


    [Header("Item & Inventory")]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] int pickupRange;
    List<Item> inventory = new List<Item>();
    [SerializeField] GameObject InventoryGUI;


    [SerializeField] GameObject achievementManagerObject;
    AchievementManager achievementManager;

    [SerializeField] public QuestSystem activeQuest;
    [SerializeField] public GameObject QuestUI;

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
        achievementManager = achievementManagerObject.GetComponent<AchievementManager>();
        achievementManager.AddObserver(this);
            
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

        // Check for player input to pick up items
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange, LayerMask.GetMask("Item"));
        bool hitOnce = false;
        foreach (var hitCollider in hitColliders)
        {
            Item item = hitCollider.gameObject.GetComponent<Item>();
            if (item != null)
            {
                //InventoryGUI.child("PickupAlert").active = true;
                //Debug.Log("Alert Player to Pickup");
                InventoryGUI.transform.Find("PickupAlert").gameObject.active = true;
                hitOnce = true;
            }
        }
        if (!hitOnce) { InventoryGUI.transform.Find("PickupAlert").gameObject.active = false; } // If no item is hit
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var hitCollider in hitColliders)
            {
                Item item = hitCollider.gameObject.GetComponent<Item>();
                if (item != null)
                {
                    PickUpItem(item);
                    InventoryGUI.transform.Find("ItemContainer").transform.Find("ItemPanel").gameObject.active = true;
                    //Debug.Log("Picked Up Item!");

                    
                    // Retrieve the "Collector" achievement
                    Achievement collectorAchievement = achievementManager.GetAchievementById("Collector");
                    if (collectorAchievement != null && !achievementManager.IsAchievementUnlocked(collectorAchievement))
                    {
                        // Unlock the "Collector" achievement
                        achievementManager.UnlockAchievement(collectorAchievement);
                        Debug.Log("Achievement Unlocked - Collector!");
                    }
                    else
                    {
                        Debug.Log("Collector Achievement already achieved.");
                    }

                    if (activeQuest.GetCurrentObjective().ObjectiveType == QuestEnums.Collect) // If quest is on an escape objective, add progress.
                    {
                        List<System.Object> specifics = new List<System.Object>(); specifics.Add("Tutorial"); specifics.Add(1);

                        this.GetComponent<Subject>().NotifyObservers(SubjectEnums.Quest, specifics);
                    }
                }
            }
        }

        // Check for player input to drop items from inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Drop the last item in the inventory (for demonstration purposes)
            if (inventory.Count > 0)
            {
                Item lastItem = inventory[inventory.Count - 1];
                DropItem(lastItem);
                InventoryGUI.transform.Find("ItemContainer").transform.Find("ItemPanel").gameObject.active = false;
                Debug.Log("Dropped Item!");
            }
        }

        // Updating Quest UI (can definitely be optimized)
        if (activeQuest != null)
        {
            if (activeQuest.IsQuestComplete() == true)
            {
                activeQuest = null;
            }

            QuestUI.transform.Find("QuestTitle").gameObject.GetComponent<TextMeshProUGUI>().text = activeQuest.QuestName;
            QuestUI.transform.Find("ObjectiveName").gameObject.GetComponent<TextMeshProUGUI>().text = activeQuest.GetCurrentObjective().ObjectiveName;
            QuestUI.transform.Find("ObjectiveDescription").gameObject.GetComponent<TextMeshProUGUI>().text = activeQuest.GetCurrentObjective().ObjectiveDescription;
            QuestUI.transform.Find("ProgressMeter").gameObject.GetComponent<TextMeshProUGUI>().text = "[" + activeQuest.GetCurrentObjective().Progress + "/" + activeQuest.GetCurrentObjective().ProgressRequired + "]";
        }
        else
        {
            QuestUI.transform.Find("QuestTitle").gameObject.GetComponent<TextMeshProUGUI>().text = "No Active Quest";
            QuestUI.transform.Find("ObjectiveName").gameObject.GetComponent<TextMeshProUGUI>().text = "Empty";
            QuestUI.transform.Find("ObjectiveDescription").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            QuestUI.transform.Find("ProgressMeter").gameObject.GetComponent<TextMeshProUGUI>().text = "";
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

    // Method to handle notification from the AchievementManager
    public void OnNotify(SubjectEnums subjectEnum, List<System.Object> parameters)
    {
        // Handle notifications here if needed
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

            if (activeQuest.GetCurrentObjective().ObjectiveType == QuestEnums.GetHurt) // If quest is on a get hurt objective, add progress.
            {
                Debug.Log("Send GetHurt Progress");

                List<System.Object> specifics = new List<System.Object>(); specifics.Add("Tutorial"); specifics.Add(1);

                this.GetComponent<Subject>().NotifyObservers(SubjectEnums.Quest, specifics);
            }
        }
        else if (other.CompareTag("Goal"))
        {
            LoadScene.Instance().MainMenuButton();
        }
        else if (other.CompareTag("QuestDestination"))
        {
            if (activeQuest.GetCurrentObjective().ObjectiveType == QuestEnums.Reach && other.gameObject.name == activeQuest.GetCurrentTarget().name) // If quest is on a reach destination objective & player touches objective, add progress.
            {
                List<System.Object> specifics = new List<System.Object>(); specifics.Add("Tutorial"); specifics.Add(1);

                this.GetComponent<Subject>().NotifyObservers(SubjectEnums.Quest, specifics);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Damage(5);

            if (activeQuest.GetCurrentObjective().ObjectiveType == QuestEnums.GetHurt) // If quest is on an escape objective, add progress.
            {
                List<System.Object> specifics = new List<System.Object>(); specifics.Add("Tutorial"); specifics.Add(1);

                this.GetComponent<Subject>().NotifyObservers(SubjectEnums.Quest, specifics);
            }
        }
    }

    public void PickUpItem(Item item)
    {
        // Add the item to the player's inventory
        inventory.Add(item);

        // Remove the item from the game world
        Destroy(item.gameObject);
    }

    public void DropItem(Item item)
    {
        // Remove the item from the player's inventory
        inventory.Remove(item);

        // Instantiate the item in the world at the player's position
        Instantiate(itemPrefab, transform.position, Quaternion.identity);
    }
}