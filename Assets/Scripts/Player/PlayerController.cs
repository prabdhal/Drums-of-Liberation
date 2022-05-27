using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  // components
  private PlayerManager manager;
  private PlayerControls controls;
  private Transform cam;
  private CharacterController controller;
  private Animator anim;

  private float horizontal;
  private float vertical;
  private Vector3 moveDir;
  private Vector3 jumpDir;

  // stats
  [SerializeField]
  private float moveSpeed = 5f;

  private void Start()
  {
    manager = PlayerManager.Instance;
    controls = PlayerControls.Instance;
    cam = Camera.main.transform;
    controller = GetComponent<CharacterController>();
    anim = GetComponentInChildren<Animator>();
  }

  private void Update()
  {
    MovementHandler();
    RotationHandler();
    AnimationHandler();
  }

  private void AnimationHandler()
  {
    anim.SetFloat(StringData.MoveSpeed, moveDir.magnitude);
    anim.SetFloat(StringData.MoveSpeedX, controls.MovementDirection.x);
    anim.SetFloat(StringData.MoveSpeedY, controls.MovementDirection.y);
    anim.SetBool(StringData.IsGrounded, controller.isGrounded);
  }

  private void MovementHandler()
  {
    horizontal = controls.MovementDirection.x;
    vertical = controls.MovementDirection.y;

    moveDir = new Vector3(horizontal, moveDir.y, vertical);
    moveDir = moveDir.x * cam.right + moveDir.z * cam.forward;
    moveDir.Normalize();
     
    controller.Move(new Vector3(moveDir.x, jumpDir.y, moveDir.z) * moveSpeed * Time.deltaTime);
  }

  private void RotationHandler()
  {
    if (moveDir.magnitude > 0.1f)
    {
      float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }
  }
}