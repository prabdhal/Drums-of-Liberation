using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // components
    private Animator anim;
    public CharacterController controller;

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
    public bool InCombat { get { return inCombat; } }
    private bool inCombat = false;
    public bool IsDead { get { return isDead; } }
    private bool isDead = false;

    public Transform popupPos;

    // stats
    public PlayerStats Stats { get { return stats; } set { stats = value; } }
    [SerializeField] PlayerStats stats;
    public StatusEffectManager StatusEffectManager { get { return statusEffectManager; } set { StatusEffectManager = value; } }
    [SerializeField] StatusEffectManager statusEffectManager;

    public EnemyManager lockOnTarget;
    public bool isDiving = false;

    [SerializeField] float inCombatStartTimer = 30f;
    private float inCombatCurrTimer = 0f;


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
        if (controller == null)
            controller = GetComponent<CharacterController>();
        statusEffectManager = GetComponentInChildren<StatusEffectManager>();

        PlayerControls.Instance.OnCycleRightEvent += IncreaseMagicIdx;
        PlayerControls.Instance.OnCycleLeftEvent += DecreaseMagicIdx;
        PlayerControls.Instance.OnPauseEvent += OnPauseMenu;

        PlayerDataManager.Instance.LoadProgress();

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
        if (!InCombat)
            stats.PassiveRegen();
        
        InCombatHandler();
        stats.Update();

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

    private void InCombatHandler()
    {
        if (inCombat)
        {
            if (inCombatCurrTimer <= 0)
            {
                inCombatCurrTimer = inCombatStartTimer;
                inCombat = false;
            }
            else
            {
                inCombatCurrTimer -= Time.deltaTime;
            }
        }
    }

    private void ResetCombatTimer()
    {
        inCombat = true;
        inCombatCurrTimer = inCombatStartTimer;
    }

    public void TakeDamage(float amount, Transform hitObj)
    {
        ResetCombatTimer();
        stats.CurrentHealth -= amount;
        stats.UpdateUI(true,false,false,false);
        GetHitDirection(hitObj);
        BloodEffect(hitObj);
    }

    private void BloodEffect(Transform hitObj)
    {
        Vector3 incomingDir = transform.position - hitObj.transform.position;
        Quaternion lookRot = Quaternion.LookRotation(incomingDir, transform.up);
        Quaternion bloodRot = Quaternion.RotateTowards(transform.rotation, lookRot, 10000f);

        Vector3 bloodOrigin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        Instantiate(GameManager.Instance.bloodEffectPrefab, bloodOrigin, bloodRot);
    }

    public bool CanUseSpell(float cost)
    {
        if (stats.CurrentMana < cost) return false;

        stats.CurrentMana -= cost;
        stats.UpdateUI(false,true,false,false);
        return true;
    }

    public bool HasEnoughStamina(float cost)
    {
        if (stats.CurrentStamina < cost) return false;

        stats.CurrentStamina -= cost;
        stats.UpdateUI(false,false,true,false);
        return true;
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

    public void GetHitDirection(Transform hitObj)
    {
        if (anim.GetBool(StringData.IsInteracting)) return;

        Vector3 incomingDir = transform.position - hitObj.position;

        float dir = Vector3.Dot(transform.forward, incomingDir);

        if (dir > 0.5f)
            anim.Play(StringData.HitB);
        else if (dir < 0.5f && dir > -0.5f)
        {
            dir = Vector3.Dot(transform.right, incomingDir);
            if (dir < 0)
                anim.Play(StringData.HitR);
            else
                anim.Play(StringData.HitL);
        }
        else if (dir < -0.5f)
            anim.Play(StringData.HitF);
    }
}
