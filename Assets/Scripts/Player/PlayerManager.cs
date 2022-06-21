using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // components
    private Animator anim;
    private CharacterController controller;

    [SerializeField]
    private CinemachineFreeLook playerFreeCam;
    [SerializeField]
    private CinemachineVirtualCamera playerLockCam;

    // state
    public bool IsGrounded { get { return controller.isGrounded; } }
    public bool IsInteracting { get; set; }
    public int CombatIdx { get; set; }
    public int MagicIdx { get { return magicIdx; } }
    private int magicIdx = 0;
    public int MaxMagicIdx { get { return maxMagicIdx; } }
    private int maxMagicIdx = 2;
    public bool IsJumping { get; set; }
    public bool TargetLock { get; set; }

    public Transform popupPos;

    // stats
    public PlayerStats Stats { get { return stats; } set { stats = value; } }
    [SerializeField]
    private PlayerStats stats;

    public EnemyManager lockOnTarget;
    public bool isDiving = false;


    #region Singleton
    public static PlayerManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    #endregion

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        PlayerControls.Instance.OnCycleRightEvent += IncreaseMagicIdx;
        PlayerControls.Instance.OnCycleLeftEvent += DecreaseMagicIdx;

        Stats.Init();
        magicIdx = 0;
    }

    private void Update()
    {
        IsInteracting = anim.GetBool(StringData.IsInteracting);
    }

    public void IncreaseMagicIdx()
    {
        magicIdx++;
        if (MagicIdx > maxMagicIdx)
            magicIdx = 0;
    }
    public void DecreaseMagicIdx()
    {
        magicIdx--;
        if (MagicIdx < 0)
            magicIdx = maxMagicIdx;
    }

    public void OnLock(Transform lookAt)
    {
        if (TargetLock)
        {
            playerLockCam.gameObject.SetActive(true);
            playerFreeCam.gameObject.SetActive(false);
            playerLockCam.LookAt = lookAt;
        }
        else
        {
            playerFreeCam.gameObject.SetActive(true);
            playerLockCam.gameObject.SetActive(false);
        }
    }
}
