using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    public TargetLockDetection targetLockDetection;
    private PlayerInputActions playerInput;
    private InputAction interact;
    private InputAction movement;
    private InputAction camera;
    private InputAction mouse;
    private InputAction mouseX;
    private InputAction mouseY;
    private InputAction sprint;
    private InputAction dive;
    private InputAction lockOn;
    private InputAction pause;

    public event OnLightAttackDel OnLightAttackEvent;
    public delegate void OnLightAttackDel();
    public event OnHeavyAttackDel OnHeavyAttackEvent;
    public delegate void OnHeavyAttackDel();
    public event OnMagicAttackDel OnMagicAttackEvent;
    public delegate void OnSpecialAttackDel();
    public event OnSpecialAttackDel OnSpecialAttackEvent;
    public delegate void OnMagicAttackDel();
    public event OnCycleRightDel OnCycleRightEvent;
    public delegate void OnCycleRightDel();
    public event OnCycleLefttDel OnCycleLeftEvent;
    public delegate void OnCycleLefttDel();
    public event OnDiveDel OnDiveEvent;
    public delegate void OnDiveDel();
    public event OnLockOnDel OnLockOnEvent;
    public delegate void OnLockOnDel();
    public event OnPauseDel OnPauseEvent;
    public delegate void OnPauseDel();
    public event OnInteractDel OnInteractEvent;
    public delegate void OnInteractDel();
    public event OnUseItemDel OnUseItemEvent;
    public delegate void OnUseItemDel();

    [Header("Tutorial Limiters")]
    public bool canMove = false;
    public bool canRotate = false;
    public bool canSprint = false;
    public bool canLightAttack = false;
    public bool canHeavyAttack = false;
    public bool canMagicAttack = false;
    public bool canLockOn = false;
    public bool canDive = false;

    public Vector2 MovementDirection { get { return _movementDirection; } }
    private Vector2 _movementDirection;
    public bool IsSprinting { get { return _isSprinting; } }
    private bool _isSprinting = false;
    public bool GetInteractingValue { get { return interacting; } }
    private bool interacting = false;

    public bool LockModeOn { get { return _lockModeOn; } }
    private bool _lockModeOn = false;

    #region Singleton
    public static PlayerControls Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        playerInput = new PlayerInputActions();
    }
    #endregion


    private void OnEnable()
    {
        movement = playerInput.Player.Move;
        camera = playerInput.Player.Camera;
        sprint = playerInput.Player.Sprint;
        dive = playerInput.Player.Dive;

        playerInput.Player.Sprint.performed += OnSprint;
        playerInput.Player.Sprint.canceled += OffSprint;
        playerInput.Player.Sprint.Enable();

        playerInput.Player.LightAttack.performed += OnLightAttack;
        playerInput.Player.LightAttack.Enable();

        playerInput.Player.HeavyAttack.performed += OnHeavyAttack;
        playerInput.Player.HeavyAttack.Enable();

        playerInput.Player.MagicAttack.performed += OnMagicAttack;
        playerInput.Player.MagicAttack.Enable();

        playerInput.Player.CycleRight.performed += OnCycleRight;
        playerInput.Player.CycleRight.Enable();

        playerInput.Player.CycleLeft.performed += OnCycleLeft;
        playerInput.Player.CycleLeft.Enable();

        playerInput.Player.MenuButton.performed += OnPause;
        playerInput.Player.MenuButton.Enable();

        playerInput.Player.Interact.started += OnInteractActive;
        playerInput.Player.Interact.performed += OnInteract;
        playerInput.Player.Interact.canceled += OnInteractDisable;
        playerInput.Player.Interact.Enable();

        playerInput.Player.UseItem.performed += OnUseItem;

        playerInput.Player.Dive.performed += OnDive;
        playerInput.Player.Dive.canceled += OnDive;
        playerInput.Player.Dive.Enable();
        playerInput.Player.LockOn.performed += OnLockOn;
        playerInput.Player.LockOn.Enable();

        playerInput.Enable();
    }

    private void OnDisable()
    {

        playerInput.Player.Sprint.performed -= OnSprint;
        playerInput.Player.Sprint.canceled -= OffSprint;
        playerInput.Player.Sprint.Disable();

        playerInput.Player.LightAttack.performed -= OnLightAttack;
        playerInput.Player.LightAttack.Disable();

        playerInput.Player.HeavyAttack.performed -= OnHeavyAttack;
        playerInput.Player.HeavyAttack.Disable();

        playerInput.Player.CycleRight.performed -= OnCycleRight;
        playerInput.Player.CycleRight.Disable();

        playerInput.Player.CycleLeft.performed -= OnCycleLeft;
        playerInput.Player.CycleLeft.Disable();

        playerInput.Player.MagicAttack.performed -= OnMagicAttack;
        playerInput.Player.MagicAttack.Disable();

        playerInput.Player.MenuButton.performed -= OnPause;
        playerInput.Player.MenuButton.Disable();

        playerInput.Player.Interact.started -= OnInteractActive;
        playerInput.Player.Interact.performed -= OnInteract;
        playerInput.Player.Interact.canceled -= OnInteractDisable;
        playerInput.Player.Interact.Disable();

        playerInput.Player.UseItem.performed -= OnUseItem;

        playerInput.Player.Dive.performed -= OnDive;
        playerInput.Player.Dive.canceled -= OnDive;
        playerInput.Player.Dive.Disable();
        playerInput.Player.LockOn.performed -= OnLockOn;
        playerInput.Player.LockOn.Enable();

        playerInput.Disable();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals(SceneNames.TutorialScene.ToString()))
        {
            FullPlayerControl(false);
        }
        else
        {
            FullPlayerControl(true);
        }

        targetLockDetection = GameObject.FindGameObjectWithTag(StringData.TargetDetection).GetComponent<TargetLockDetection>();
    }

    private void Update()
    {
        if (canMove)
            _movementDirection = movement.ReadValue<Vector2>();
        else
            _movementDirection = new Vector2(0, 0);

        if (SceneManager.GetActiveScene().name.Equals(SceneNames.TutorialScene.ToString()))
        {
            if (DialogueManager.Instance.dialoguePanel.activeInHierarchy)
                FullPlayerControl(false);
            else if (!SceneManager.GetActiveScene().name.Equals(SceneNames.TutorialScene.ToString()))
                FullPlayerControl(true);
        }
    }

    public void FullPlayerControl(bool allow)
    {
        canMove = allow;
        canRotate = allow;
        canSprint = allow;
        canLightAttack = allow;
        canHeavyAttack = allow;
        canMagicAttack = allow;
        canLockOn = allow;
        canDive = allow;
    }

    private void OnInteractActive(InputAction.CallbackContext obj)
    {
        interacting = true;
    }

    private void OnInteract(InputAction.CallbackContext obj)
    {
        OnInteractEvent?.Invoke();
    }

    private void OnUseItem(InputAction.CallbackContext obj)
    {
        Debug.Log("Invoke OnUseItem");
        OnUseItemEvent?.Invoke();
    }

    private void OnInteractDisable(InputAction.CallbackContext obj)
    {
        interacting = false;
    }

    private void OnSprint(InputAction.CallbackContext obj)
    {
        if (!canSprint) return;

        _isSprinting = true;
    }

    private void OffSprint(InputAction.CallbackContext obj)
    {
        _isSprinting = false;
    }

    private void OnLightAttack(InputAction.CallbackContext obj)
    {
        if (!canLightAttack) return;
        if (PlayerManager.Instance.IsGrounded == false || PlayerManager.Instance.IsInteracting) return;

        OnLightAttackEvent?.Invoke();
    }

    private void OnHeavyAttack(InputAction.CallbackContext obj)
    {
        if (!canHeavyAttack) return;
        if (PlayerManager.Instance.IsGrounded == false || PlayerManager.Instance.IsInteracting) return;

        OnHeavyAttackEvent?.Invoke();
    }

    private void OnMagicAttack(InputAction.CallbackContext obj)
    {
        if (!canMagicAttack) return;
        if (PlayerManager.Instance.IsGrounded == false || PlayerManager.Instance.IsInteracting) return;

        OnMagicAttackEvent?.Invoke();
    }

    private void OnCycleRight(InputAction.CallbackContext obj)
    {
        OnCycleRightEvent?.Invoke();
    }

    private void OnCycleLeft(InputAction.CallbackContext obj)
    {
        OnCycleLeftEvent?.Invoke();
    }

    private void OnDive(InputAction.CallbackContext obj)
    {
        if (!canDive) return;
        if (PlayerManager.Instance.IsGrounded == false || PlayerManager.Instance.IsInteracting) return;

        OnDiveEvent?.Invoke();
    }

    private void OnLockOn(InputAction.CallbackContext obj)
    {
        if (!canLockOn) return;
        GameObject target = targetLockDetection.GetClosestTarget();
        if (target != null && PlayerManager.Instance.TargetLock == false)
        {
            _lockModeOn = true;
            PlayerManager.Instance.TargetLock = true;
            PlayerManager.Instance.lockOnTarget = target.GetComponent<EnemyManager>();
            PlayerManager.Instance.OnLock(target.transform);
        }
        else
        {
            _lockModeOn = false;
            PlayerManager.Instance.TargetLock = false;
            PlayerManager.Instance.lockOnTarget = null;
            PlayerManager.Instance.OnLock(null);
        }

        OnLockOnEvent?.Invoke();
    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        OnPauseEvent?.Invoke();
    }
}
