using Cinemachine;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // components
    private Animator anim;
    public CharacterController controller;

    [SerializeField] CinemachineFreeLook playerFreeCam;
    [SerializeField] CinemachineVirtualCamera playerLockCam;
    [SerializeField] float yCamSpeed = 2f;
    [SerializeField] float xCamSpeed = 300f;

    public Transform levelUpEffectOrigin;

    [SerializeField] TextMeshProUGUI goldText;
    public float Gold { get; set; }

    // state
    public bool IsGrounded { get { return controller.isGrounded; } }
    public bool IsSprinting { get; set; }
    public bool IsInteracting { get; set; }
    public bool IsLightAttacking { get; set; }
    public bool IsHeavyAttacking { get; set; }
    public bool IsMagicAttacking { get; set; }
    public int CombatIdx { get; set; }
    public int ComboIdx { get; set; }
    public int MagicIdx { get { return magicIdx; } }
    private int magicIdx = 0;
    public int MaxMagicIdx { get { return maxMagicIdx; } }
    private int maxMagicIdx = 0;
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

    [Header("Player KnockBack Values")]
    private bool warchiefKnockBack = false;
    private bool barbarianKnockBack = false;
    private Vector3 knockBackDir = Vector3.zero;
    [SerializeField] float maxKnockBackDis = 5f;
    [SerializeField] float maxBarbarianGiantKnockBackDis = 20f;
    private Vector3 curKnockBackDis;
    [SerializeField] float maxWarchiefKnockBackTimer = 1f;
    [SerializeField] float maxBarbarianGiantKnockBackTimer = 3f;
    private float curKnockBackTimer = 1f;
    [SerializeField] float warchiefKnockBackSpeed = 200f;
    [SerializeField] float barbarianKnockBackSpeed = 50f;
    [SerializeField] LayerMask knockBackLayer;


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
        if (goldText == null)
            goldText = GameObject.FindGameObjectWithTag(StringData.GoldText).GetComponent<TextMeshProUGUI>();

        anim = GetComponentInChildren<Animator>();
        if (controller == null)
            controller = GetComponent<CharacterController>();
        statusEffectManager = GetComponentInChildren<StatusEffectManager>();

        Stats.OnLevelUpEvent += LevelUpEffect;

        PlayerControls.Instance.OnCycleRightEvent += IncreaseMagicIdx;
        PlayerControls.Instance.OnCycleLeftEvent += DecreaseMagicIdx;
        PlayerControls.Instance.OnPauseEvent += OnPauseMenu;

        PlayerDataManager.Instance.LoadProgress();
        MenuDataManager.Instance.LoadProgress();
        Stats.Init();
        magicIdx = 0;
        UpdateGoldTextUI();
    }

    private void Update()
    {
        CameraHandler();

        if (stats.CurrentHealth <= 0)
        {
            Dead();
            return;
        }
        if (!InCombat)
            stats.PassiveRegen();

        if (warchiefKnockBack)
            WarChiefKickBackHandler(knockBackDir);
        if (barbarianKnockBack)
            BarbarianGiantKickBackHandler(knockBackDir);

        if (stats.playerLevel >= 3)
            maxMagicIdx = 1;

        CanSprint();
        InCombatHandler();
        stats.Update();

        IsInteracting = anim.GetBool(StringData.IsInteracting);
    }

    private void CameraHandler()
    {
        if (!PlayerControls.Instance.canRotate)
        {
            playerFreeCam.m_YAxis.m_MaxSpeed = 0f;
            playerFreeCam.m_XAxis.m_MaxSpeed = 0f;
        }
        else
        {
            playerFreeCam.m_YAxis.m_MaxSpeed = yCamSpeed;
            playerFreeCam.m_XAxis.m_MaxSpeed = xCamSpeed;
        }
    }

    private void CanSprint()
    {
        if (PlayerControls.Instance.IsSprinting && !IsInteracting && PlayerControls.Instance.MovementDirection.x != 0 ||
            PlayerControls.Instance.IsSprinting && !IsInteracting && PlayerControls.Instance.MovementDirection.y != 0)
        {
            if (HasEnoughStamina(0.01f))
                IsSprinting = true;
            else
                IsSprinting = false;
        }
        else
            IsSprinting = false;

    }

    private void LevelUpEffect()
    {
        Instantiate(GameManager.Instance.levelUpEffectPrefab, levelUpEffectOrigin.position, Quaternion.identity);
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

    public void TakeDamage(float amount, Transform hitObj = null)
    {
        ResetCombatTimer();
        stats.CurrentHealth -= amount;
        stats.UpdateUI(true, false, false, false);
        GetHitDirection(hitObj);
        BloodEffect(hitObj);
    }

    public void ApplyWarChiefKickBack(Transform hitObj)
    {
        anim.SetBool(StringData.IsInteracting, true);
        anim.Play(StringData.KnockBack);
        anim.SetBool(StringData.IsKnockedBack, true);
        Vector3 incomingDir = transform.position - hitObj.position;
        knockBackDir = incomingDir;
        curKnockBackDis = transform.position;
        curKnockBackTimer = maxWarchiefKnockBackTimer;
        warchiefKnockBack = true;
    }

    public void ApplyBarbarianGiantKickBack(Transform hitObj)
    {
        anim.SetBool(StringData.IsInteracting, true);
        anim.Play(StringData.KnockBack);
        anim.SetBool(StringData.IsKnockedBack, true);
        Vector3 incomingDir = transform.position - hitObj.position;
        knockBackDir = incomingDir;
        curKnockBackDis = transform.position;
        curKnockBackTimer = maxBarbarianGiantKnockBackTimer;
        barbarianKnockBack = true;
    }

    private void WarChiefKickBackHandler(Vector3 knockbackDir)
    {
        RaycastHit hit;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        float maxDistance = 2f;

        Debug.DrawRay(origin, knockbackDir * maxDistance, Color.blue);
        if (Physics.Raycast(origin, knockbackDir, out hit, maxDistance, knockBackLayer))
        {
            Debug.Log("Collided with: " + hit.transform.name);
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsKnockedBack, false);

            TempStatDebuffEffect effect = new TempStatDebuffEffect(20f, 2f, 1f, TempStatDebuffType.Stun);
            StatusEffectManager.ApplyDebuffEffects(effect);
            warchiefKnockBack = false;
        }
        if (curKnockBackTimer <= 0)
        {
            Debug.Log("Timer is 0");
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsKnockedBack, false);
            warchiefKnockBack = false;
        }
        float distance = Vector3.Distance(transform.position, curKnockBackDis);
        if (distance >= maxKnockBackDis)
        {
            Debug.Log("Distance is 0");
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsKnockedBack, false);
            warchiefKnockBack = false;
        }

        curKnockBackTimer -= Time.deltaTime;
        controller.Move(knockbackDir * barbarianKnockBackSpeed * Time.deltaTime);
    }

    private void BarbarianGiantKickBackHandler(Vector3 knockbackDir)
    {
        RaycastHit hit;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        float maxDistance = 2f;

        Debug.DrawRay(origin, knockbackDir * maxDistance, Color.blue);
        if (Physics.Raycast(origin, knockbackDir, out hit, maxDistance, knockBackLayer))
        {
            Debug.Log("Collided with: " + hit.transform.name);
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsKnockedBack, false);

            TempStatDebuffEffect effect = new TempStatDebuffEffect(20f, 2f, 1f, TempStatDebuffType.Stun);
            StatusEffectManager.ApplyDebuffEffects(effect);
            barbarianKnockBack = false;
        }
        if (curKnockBackTimer <= 0)
        {
            Debug.Log("Timer is 0");
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsKnockedBack, false);
            barbarianKnockBack = false;
        }
        float distance = Vector3.Distance(transform.position, curKnockBackDis);
        if (distance >= maxBarbarianGiantKnockBackDis)
        {
            Debug.Log("Distance is 0");
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsKnockedBack, false);
            barbarianKnockBack = false;
        }

        curKnockBackTimer -= Time.deltaTime;
        controller.Move(knockbackDir * barbarianKnockBackSpeed * Time.deltaTime);
    }

    private void BloodEffect(Transform hitObj)
    {
        if (hitObj == null)
            return;

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
        stats.UpdateUI(false, true, false, false);
        return true;
    }

    public bool HasEnoughStamina(float cost)
    {
        if (stats.CurrentStamina < cost && stats.playerLevel < 5) return false;

        if (stats.playerLevel >= 5)
            cost = 0;

        stats.CurrentStamina -= cost;
        stats.UpdateUI(false, false, true, false);
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

    public void FullRestore()
    {
        statusEffectManager.ClearStatusEffects();
        statusEffectManager.ClearDebuffEffects();
        statusEffectManager.ClearTempModifiersEffects();

        stats.FullRestore();
    }

    private void Dead()
    {
        isDead = true;
        IsInteracting = true;
        tag = StringData.Untagged;
        anim.Play(StringData.Dead);
        FullRestore();
        if (PlayerDataManager.TutorialFinished)
            GameManager.Instance.LoadScene(SceneNames.SafeZoneSceneOne.ToString());
        else
            GameManager.Instance.ResetScene();
    }

    public void GetHitDirection(Transform hitObj)
    {
        if (anim.GetBool(StringData.IsInteracting)) return;

        if (hitObj == null)
        {
            anim.Play(StringData.HitF2);
            return;
        }

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

    public void AddPlayerGold(float gold)
    {
        Gold += gold;
        UpdateGoldTextUI();
    }

    public void UpdateGoldTextUI()
    {
        goldText.text = Gold.ToString();
    }
}
