using UnityEngine;

public class EnemyLockOn : MonoBehaviour
{
  private Transform currentTarget;
  private Animator anim;

  [SerializeField]
  private LayerMask targetLayers;
  [SerializeField]
  private Transform enemyTarget_Locator;

  [Tooltip("StateDrivenMethod for Switching Cameras")]
  [SerializeField]
  private Animator cinemachineAnimator;

  [Header("Settings")]
  [SerializeField]
  private bool zeroVert_Look;
  [SerializeField]
  private float noticeZone = 10;
  [SerializeField]
  private float lookAtSmoothing = 2;
  [Tooltip("Angle_Degree")]
  [SerializeField]
  private float maxNoticeAngle = 60;
  [SerializeField]
  private float crossHair_Scale = 0.1f;


  private Transform cam;
  private bool enemyLocked;
  private float currentYOffset;
  private Vector3 pos;

  [SerializeField]
  private CameraFollow camFollow;
  [SerializeField]
  private Transform lockOnCanvas;
  private PlayerController playeController;


  private void Start()
  {
    playeController = GameObject.FindGameObjectWithTag(StringData.PlayerTag).GetComponent<PlayerController>();
    anim = GetComponent<Animator>();
    cam = Camera.main.transform;
    lockOnCanvas.gameObject.SetActive(false);
    PlayerControls.Instance.OnLockOnEvent += OnLock;
  }

  private void Update()
  {
    camFollow.lockedTarget = enemyLocked;
    //playerController.lockMovement = enemyLocked;

    if (enemyLocked)
    {
      if (!TargetOnRange()) 
        ResetTarget();
      LookAtTarget();
    }
  }

  private void OnLock()
  {
    if (currentTarget)
    {
      //If there is already a target, Reset.
      ResetTarget();
      return;
    }

    if (currentTarget = ScanNearBy())
      FoundTarget();
    else
      ResetTarget();
  }


  private void FoundTarget()
  {
    lockOnCanvas.gameObject.SetActive(true);
    anim.SetLayerWeight(1, 1);
    cinemachineAnimator.Play("TargetCamera");
    enemyLocked = true;
  }

  void ResetTarget()
  {
    lockOnCanvas.gameObject.SetActive(false);
    currentTarget = null;
    enemyLocked = false;
    anim.SetLayerWeight(1, 0);
    cinemachineAnimator.Play("FollowCamera");
  }


  private Transform ScanNearBy()
  {
    Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
    float closestAngle = maxNoticeAngle;
    Transform closestTarget = null;
    if (nearbyTargets.Length <= 0) return null;

    for (int i = 0; i < nearbyTargets.Length; i++)
    {
      Vector3 dir = nearbyTargets[i].transform.position - cam.position;
      dir.y = 0;
      float _angle = Vector3.Angle(cam.forward, dir);

      if (_angle < closestAngle)
      {
        closestTarget = nearbyTargets[i].transform;
        closestAngle = _angle;
      }
    }

    if (!closestTarget) return null;
    float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
    float h2 = closestTarget.localScale.y;
    float h = h1 * h2;
    float half_h = (h / 2) / 2;
    currentYOffset = h - half_h;
    if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
    Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
    if (Blocked(tarPos)) return null;
    return closestTarget;
  }

  bool Blocked(Vector3 t)
  {
    RaycastHit hit;
    if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit))
    {
      if (!hit.transform.CompareTag("Enemy")) return true;
    }
    return false;
  }

  bool TargetOnRange()
  {
    float dis = (transform.position - pos).magnitude;
    if (dis / 2 > noticeZone) return false; else return true;
  }

  private void LookAtTarget()
  {
    if (currentTarget == null)
    {
      ResetTarget();
      return;
    }
    pos = currentTarget.position + new Vector3(0, currentYOffset, 0);
    lockOnCanvas.position = pos;
    lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale);

    enemyTarget_Locator.position = pos;
    Vector3 dir = currentTarget.position - transform.position;
    dir.y = 0;
    Quaternion rot = Quaternion.LookRotation(dir);
    transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing);
  }

  private void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(transform.position, noticeZone);
  }
}
