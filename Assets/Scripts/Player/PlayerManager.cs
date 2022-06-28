using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // components
    private Animator anim;
    private CharacterController controller;

    [SerializeField] CinemachineFreeLook playerFreeCam;
    [SerializeField] CinemachineVirtualCamera playerLockCam;

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
    public bool OnPause { get { return onPause; } }
    private bool onPause = false;
    public bool IsDead { get { return isDead; }  }
    private bool isDead = false;

    public Transform popupPos;

    // stats
    public PlayerStats Stats { get { return stats; } set { stats = value; } }
    [SerializeField] PlayerStats stats;
    public StatusEffectManager StatusEffectManager { get { return statusEffectManager;} set { StatusEffectManager = value; } }
    [SerializeField] StatusEffectManager statusEffectManager;

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
        statusEffectManager = GetComponentInChildren<StatusEffectManager>();

        PlayerControls.Instance.OnCycleRightEvent += IncreaseMagicIdx;
        PlayerControls.Instance.OnCycleLeftEvent += DecreaseMagicIdx;
        PlayerControls.Instance.OnPauseEvent += OnPauseMenu;

        Stats.Init();
        magicIdx = 0;
    }

    private void Update()
    {
        if (stats.CurrentHealth <= 0)
        {
            Dead();
            return;
        }

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

    private void OnPauseMenu()
    {
        onPause = !onPause;

        if (onPause)
            GameMenuManager.Instance.PauseGame();
        else
            GameMenuManager.Instance.ResumeGame();
    }

    private void Dead()
    {
        isDead = true;
        IsInteracting = true;
        tag = StringData.Untagged;
        anim.Play(StringData.Dead);
        GameManager.Instance.ResetScene();
    }
}
