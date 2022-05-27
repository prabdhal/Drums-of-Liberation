using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
  private PlayerInputActions playerInput;
  private InputAction interact;
  private InputAction movement;
  private InputAction mouse;
  private InputAction mouseX;
  private InputAction mouseY;
  private InputAction sprint;
  private InputAction zoom;
  private InputAction dive;
  private InputAction lockOn;
  private InputAction pause;

  private PlayerManager PlayerManager;

  public event OnJumpDel OnJumpEvent;
  public delegate void OnJumpDel();
  public event OnLightAttackDel OnLightAttackEvent;
  public delegate void OnLightAttackDel();
  public event OnHeavyAttackDel OnHeavyAttackEvent;
  public delegate void OnHeavyAttackDel();
  public event OnSpecialAttackDel OnSpecialAttackEvent;
  public delegate void OnSpecialAttackDel();
  public event OnDiveDel OnDiveEvent;
  public delegate void OnDiveDel();
  public event OnLockOnDel OnLockOnEvent;
  public delegate void OnLockOnDel();
  public event OnPauseDel OnPauseEvent;
  public delegate void OnPauseDel();
  public event OnInteractDel OnInteractEvent;
  public delegate void OnInteractDel();

  public Vector2 MovementDirection { get { return _movementDirection; } }
  private Vector2 _movementDirection;
  public Vector2 RotationDirection { get { return _rotationDirection; } }
  private Vector2 _rotationDirection;
  public bool IsSprinting { get { return sprint.ReadValue<float>() > 0; } }
  public bool GetInteractingValue { get { return interacting; } }
  private bool interacting = false;


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
    sprint = playerInput.Player.Sprint;
    dive = playerInput.Player.Dive;

    playerInput.Player.Jump.performed += OnJump;
    playerInput.Player.Jump.Enable();

    playerInput.Player.Interact.performed += OnInteract;
    playerInput.Player.Interact.Enable();

    playerInput.Player.LightAttack.performed += OnLightAttack;
    playerInput.Player.LightAttack.Enable();
    playerInput.Player.HeavyAttack.performed += OnHeavyAttack;
    playerInput.Player.HeavyAttack.Enable();
    playerInput.Player.Interact.started += OnInteractActive;
    playerInput.Player.Interact.canceled += OnInteractDisable;
    playerInput.Player.Dive.performed += OnDive;
    playerInput.Player.Dive.canceled += OnDive;
    playerInput.Player.Interact.Enable();

    playerInput.Enable();
  }

  private void OnDisable()
  {
    playerInput.Player.Jump.performed -= OnJump;
    playerInput.Player.Jump.Disable();

    playerInput.Player.Interact.performed -= OnInteract;
    playerInput.Player.Interact.Disable();

    playerInput.Disable();
  }

  private void Update()
  {
    _movementDirection = movement.ReadValue<Vector2>();
  }

  private void OnJump(InputAction.CallbackContext context)
  {
    OnJumpEvent?.Invoke();
  }

  private void OnInteract(InputAction.CallbackContext context)
  {
    Debug.Log("Interact");
  }
  private void OnInteractActive(InputAction.CallbackContext obj)
  {
    Debug.Log("Interacting is TRUE");
    interacting = true;
  }

  private void OnInteractDisable(InputAction.CallbackContext obj)
  {
    Debug.Log("Interacting is FALSE");
    interacting = false;
  }

  private void OnLightAttack(InputAction.CallbackContext obj)
  {
    if (PlayerManager.Instance.IsGrounded == false || PlayerManager.Instance.IsInteracting) return;

    OnLightAttackEvent?.Invoke();
    Debug.Log("Light Attack Event Invoked!");
  }

  private void OnHeavyAttack(InputAction.CallbackContext obj)
  {
    if (PlayerManager.Instance.IsGrounded == false || PlayerManager.Instance.IsInteracting) return;

    OnHeavyAttackEvent?.Invoke();
    Debug.Log("Heavy Attack Event Invoked!");
  }

  private void OnSpecialAttack(InputAction.CallbackContext obj)
  {
    if (PlayerManager.Instance.IsGrounded == false || PlayerManager.Instance.IsInteracting) return;

    OnSpecialAttackEvent?.Invoke();
    Debug.Log("Special Attack Event Invoked!");
  }

  private void OnDive(InputAction.CallbackContext obj)
  {
    if (PlayerManager.Instance.IsGrounded == false || PlayerManager.Instance.IsInteracting) return;

    OnDiveEvent?.Invoke();
    Debug.Log("On Dive Event Invoked!");
  }

  private void OnLockOn(InputAction.CallbackContext obj)
  {
    OnLockOnEvent?.Invoke();
    Debug.Log("On Lock On Event Invoked!");
  }
}
