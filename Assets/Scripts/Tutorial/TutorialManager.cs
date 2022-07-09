using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    #region Singleton
    public static TutorialManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }
    #endregion

    [SerializeField] EndSceneTrigger endSceneTrigger;

    [HideInInspector] public bool movementCompleteTrigger = false;
    [HideInInspector] public bool sprintCompleteTrigger = false;
    [HideInInspector] public bool diveCompleteTrigger = false;
    [HideInInspector] public bool lightAttackCompleteTrigger = false;
    [HideInInspector] public bool heavyAttackCompleteTrigger = false;
    [HideInInspector] public bool lockOnTargetCompleteTrigger = false;
    [HideInInspector] public bool magicAttackCompleteTrigger = false;
    [HideInInspector] public bool sparCombatCompleteTrigger = false;
    [HideInInspector] public bool tutorialComplete = false;

    public delegate void MovementTutorialComplete();
    public event MovementTutorialComplete OnMovementTutorialComplete;
    public delegate void SprintTutorialComplete();
    public event SprintTutorialComplete OnSprintTutorialComplete;
    public delegate void DiveTutorialComplete();
    public event DiveTutorialComplete OnDiveTutorialComplete;
    public delegate void LightAttackTutorialComplete();
    public event LightAttackTutorialComplete OnLightAttackTutorialComplete;
    public delegate void HeavyAttackTutorialComplete();
    public event HeavyAttackTutorialComplete OnHeavyAttackTutorialComplete;
    public delegate void LockOnTutorialComplete();
    public event LockOnTutorialComplete OnLockOnTutorialComplete;
    public delegate void MagicAttackTutorialComplete();
    public event MagicAttackTutorialComplete OnMagicAttackTutorialComplete;
    public delegate void SparCombatTutorialComplete();
    public event SparCombatTutorialComplete OnSparCombatTutorialComplete;
    public delegate void FinalTutorialComplete();
    public event FinalTutorialComplete OnFinalTutorialComplete;

    [SerializeField] TutorialDummyManager dummy;
    public TutorialDummyManager sparrDummy;


    private void Start()
    {
        movementCompleteTrigger = false;
        sprintCompleteTrigger = false;
        diveCompleteTrigger = false;
        lightAttackCompleteTrigger = false;
        heavyAttackCompleteTrigger = false;
        lockOnTargetCompleteTrigger = false;
        magicAttackCompleteTrigger = false;
        sparCombatCompleteTrigger = false;
    }

    private void Update()
    {
        AllowPlayerControlsBasedOnProgression();

        if (!movementCompleteTrigger && CheckForPlayerMovement())
        {
            Invoke("InvokeMovementTutorialComplete", 1f);
            MovementTutorialCompleted();
        }
        if (!sprintCompleteTrigger && CheckForPlayerSprint())
        {
            Invoke("InvokeSprintTutorialComplete", 1f);
            SprintTutorialCompleted();
        }
        if (!diveCompleteTrigger && CheckForPlayerDive())
        {
            Invoke("InvokeDiveTutorialComplete", 1f);
            DiveTutorialCompleted();
        }
        if (!lightAttackCompleteTrigger && CheckForPlayerLightAttack())
        {
            Invoke("InvokeLightAttackTutorialComplete", 1f);
            LightAttackTutorialCompleted();
        }
        if (!heavyAttackCompleteTrigger && CheckForPlayerHeavyAttack())
        {
            Invoke("InvokeHeavyAttackTutorialComplete", 1f);
            HeavyAttackTutorialCompleted();
        }
        if (!lockOnTargetCompleteTrigger && CheckForPlayerLockOn())
        {
            Invoke("InvokeLockOnTutorialComplete", 1f);
            LockOnTutorialCompleted();
        }
        if (!magicAttackCompleteTrigger && CheckForPlayerMagicAttack())
        {
            Invoke("InvokeMagicAttackTutorialComplete", 1f);
            MagicAttackTutorialCompleted();
        }
        if (!sparCombatCompleteTrigger && CheckForPlayerSparCombat())
        {
            Invoke("InvokeSparCombatTutorialComplete", 2f);
            SparCombatTutorialCompleted();
        }
        if (tutorialComplete)
        {
            Invoke("InvokeFinalTutorialComplete", 1f);
        }
    }

    #region Invoker 
    private void InvokeMovementTutorialComplete()
    {
        OnMovementTutorialComplete();
    }

    private void InvokeSprintTutorialComplete()
    {
        OnSprintTutorialComplete();
    }

    private void InvokeDiveTutorialComplete()
    {
        OnDiveTutorialComplete();
    }

    private void InvokeLightAttackTutorialComplete()
    {
        OnLightAttackTutorialComplete();
    }

    private void InvokeHeavyAttackTutorialComplete()
    {
        OnHeavyAttackTutorialComplete();
    }

    private void InvokeLockOnTutorialComplete()
    {
        OnLockOnTutorialComplete();
    }

    private void InvokeMagicAttackTutorialComplete()
    {
        OnMagicAttackTutorialComplete();
    }

    private void InvokeSparCombatTutorialComplete()
    {
        OnSparCombatTutorialComplete();
    }

    private void InvokeFinalTutorialComplete()
    {
        OnFinalTutorialComplete();
    }
    #endregion

    #region Check Conditions

    private bool CheckForPlayerMovement()
    {
        if (!PlayerControls.Instance.canMove) return false;

        if (PlayerControls.Instance.MovementDirection.x > 0f && PlayerControls.Instance.MovementDirection.y > 0f)
            return true;
        return false;
    }

    private bool CheckForPlayerSprint()
    {
        if (!PlayerControls.Instance.canSprint) return false;

        if (PlayerControls.Instance.MovementDirection.x > 0f && PlayerControls.Instance.IsSprinting ||
            PlayerControls.Instance.MovementDirection.y > 0f && PlayerControls.Instance.IsSprinting)
            return true;
        return false;
    }

    private bool CheckForPlayerDive()
    {
        if (!PlayerControls.Instance.canDive) return false;

        if (PlayerControls.Instance.MovementDirection.x > 0f && PlayerManager.Instance.isDiving ||
            PlayerControls.Instance.MovementDirection.y > 0f && PlayerManager.Instance.isDiving)
            return true;
        return false;
    }

    private bool CheckForPlayerLightAttack()
    {
        if (!PlayerControls.Instance.canLightAttack) return false;

        if (PlayerManager.Instance.ComboIdx == 3 && PlayerManager.Instance.IsLightAttacking)
            return true;
        return false;
    }

    private bool CheckForPlayerHeavyAttack()
    {
        if (!PlayerControls.Instance.canHeavyAttack) return false;

        if (PlayerManager.Instance.ComboIdx == 3 && PlayerManager.Instance.IsHeavyAttacking)
            return true;
        return false;
    }

    private bool CheckForPlayerLockOn()
    {
        if (!PlayerControls.Instance.canLockOn) return false;

        if (PlayerControls.Instance.LockModeOn)
            return true;
        return false;
    }

    private bool CheckForPlayerMagicAttack()
    {
        if (!PlayerControls.Instance.canMagicAttack) return false;

        if (PlayerManager.Instance.IsMagicAttacking)
            return true;
        return false;
    }

    private bool CheckForPlayerSparCombat()
    {
        if (sparrDummy.Stats.CurrentHealth <= 0)
            return true;
        return false;
    }

    #endregion

    #region Triggers

    private void MovementTutorialCompleted()
    {
        movementCompleteTrigger = true;
    }

    private void SprintTutorialCompleted()
    {
        sprintCompleteTrigger = true;
    }

    private void DiveTutorialCompleted()
    {
        diveCompleteTrigger = true;
    }

    private void LightAttackTutorialCompleted()
    {
        lightAttackCompleteTrigger = true;
    }

    private void HeavyAttackTutorialCompleted()
    {
        heavyAttackCompleteTrigger = true;
    }

    private void LockOnTutorialCompleted()
    {
        lockOnTargetCompleteTrigger = true;
    }

    private void MagicAttackTutorialCompleted()
    {
        magicAttackCompleteTrigger = true;
    }

    private void SparCombatTutorialCompleted()
    {
        sparCombatCompleteTrigger = true;
    }

    #endregion

    private void AllowPlayerControlsBasedOnProgression()
    {
        if (endSceneTrigger.initiateNextScene) return;

        if (movementCompleteTrigger && !DialogueManager.Instance.dialoguePanel.activeInHierarchy)
        {
            PlayerControls.Instance.canMove = true;
            PlayerControls.Instance.canRotate = true;
        }
        if (sprintCompleteTrigger && !DialogueManager.Instance.dialoguePanel.activeInHierarchy)
        {
            PlayerControls.Instance.canSprint = true;
            PlayerControls.Instance.canRotate = true;
        }
        if (diveCompleteTrigger && !DialogueManager.Instance.dialoguePanel.activeInHierarchy)
        {
            PlayerControls.Instance.canDive = true;
            PlayerControls.Instance.canRotate = true;
        }
        if (lightAttackCompleteTrigger && !DialogueManager.Instance.dialoguePanel.activeInHierarchy)
        {
            PlayerControls.Instance.canLightAttack = true;
            PlayerControls.Instance.canRotate = true;
        }
        if (heavyAttackCompleteTrigger && !DialogueManager.Instance.dialoguePanel.activeInHierarchy)
        {
            PlayerControls.Instance.canHeavyAttack = true;
            PlayerControls.Instance.canRotate = true;
        }
        if (lockOnTargetCompleteTrigger && !DialogueManager.Instance.dialoguePanel.activeInHierarchy)
        {
            PlayerControls.Instance.canLockOn = true;
            PlayerControls.Instance.canRotate = true;
        }
        if (magicAttackCompleteTrigger && !DialogueManager.Instance.dialoguePanel.activeInHierarchy)
        {
            PlayerControls.Instance.canMagicAttack = true;
            PlayerControls.Instance.canRotate = true;
        }
    }
}
