using TMPro;
using UnityEngine;

public class TutorialGuy : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Dialogue movementTrainingDialogue;
    [SerializeField] Dialogue sprintTrainingDialogue;
    [SerializeField] Dialogue diveTrainingDialogue;
    [SerializeField] Dialogue lightAttackTrainingDialogue;
    [SerializeField] Dialogue heavyAttackTrainingDialogue;
    [SerializeField] Dialogue lockOnTargetTrainingDialogue;
    [SerializeField] Dialogue magicAttackTrainingDialogue;
    [SerializeField] Dialogue sparCombatTrainingDialogue;
    [SerializeField] Dialogue finalTrainingDialogue;

    [SerializeField] TextMeshProUGUI movementTitleTutorial;
    [SerializeField] TextMeshProUGUI movementTutorial;
    [SerializeField] TextMeshProUGUI sprintTitleTutorial;
    [SerializeField] TextMeshProUGUI sprintTutorial;
    [SerializeField] TextMeshProUGUI diveTitleTutorial;
    [SerializeField] TextMeshProUGUI diveTutorial;
    [SerializeField] TextMeshProUGUI lightAttackTitleTutorial;
    [SerializeField] TextMeshProUGUI lightAttackTutorial;
    [SerializeField] TextMeshProUGUI heavyAttackTitleTutorial;
    [SerializeField] TextMeshProUGUI heavyAttackTutorial;
    [SerializeField] TextMeshProUGUI LockOnTitleTutorial;
    [SerializeField] TextMeshProUGUI LockOnTutorial;
    [SerializeField] TextMeshProUGUI magicAttackTitleTutorial;
    [SerializeField] TextMeshProUGUI magicAttackTutorial;

    [SerializeField] Color32 activeColor;
    [SerializeField] Color32 completeColor;
    [SerializeField] Color32 todoColor;

    [SerializeField] bool init = true;
    [SerializeField] float startDialogueTimer = 2f;
    private float curDialogueTimer = 2f;

    [SerializeField] Animator leftDoorAnim;
    [SerializeField] Animator rightDoorAnim;


    private void Start()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        init = true;
        curDialogueTimer = startDialogueTimer;

        TutorialManager.Instance.OnMovementTutorialComplete += SprintTrainingTalk;
        TutorialManager.Instance.OnSprintTutorialComplete += DiveTrainingTalk;
        TutorialManager.Instance.OnDiveTutorialComplete += LightAttackTrainingTalk;
        TutorialManager.Instance.OnLightAttackTutorialComplete += HeavyAttackTrainingTalk;
        TutorialManager.Instance.OnHeavyAttackTutorialComplete += LockOnTrainingTalk;
        TutorialManager.Instance.OnLockOnTutorialComplete += MagicAttackTrainingTalk;
        TutorialManager.Instance.OnMagicAttackTutorialComplete += SparCombatTrainingTalk;
        TutorialManager.Instance.OnSparCombatTutorialComplete += FinalTrainingTalk;

        InitTutorialToDoList();
    }

    private void Update()
    {
        if (curDialogueTimer <= 0 && init)
        {
            init = false;
            MovementTrainingTalk();
        }
        else if (init)
            curDialogueTimer -= Time.deltaTime;
    }

    private void MovementTrainingTalk()
    {
        anim.CrossFade(StringData.IsTalking01, 0.2f);
        DialogueManager.Instance.AddNewDialogue(movementTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerMovement;
    }

    private void SprintTrainingTalk()
    {
        movementTitleTutorial.color = completeColor;
        movementTutorial.color = completeColor;
        sprintTitleTutorial.color = activeColor;
        sprintTutorial.color = activeColor;
        anim.CrossFade(StringData.IsTalking02, 0.2f);
        DialogueManager.Instance.AddNewDialogue(sprintTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerSprint;
    }

    private void DiveTrainingTalk()
    {
        sprintTitleTutorial.color = completeColor;
        sprintTutorial.color = completeColor;
        diveTitleTutorial.color = activeColor;
        diveTutorial.color = activeColor;
        anim.CrossFade(StringData.IsTalking02, 0.2f);
        DialogueManager.Instance.AddNewDialogue(diveTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerDive;
    }

    private void LightAttackTrainingTalk()
    {
        diveTitleTutorial.color = completeColor;
        diveTutorial.color = completeColor;
        lightAttackTutorial.color = activeColor;
        lightAttackTitleTutorial.color = activeColor;
        anim.CrossFade(StringData.IsTalking02, 0.2f);
        DialogueManager.Instance.AddNewDialogue(lightAttackTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerLightAttack;
    }

    private void HeavyAttackTrainingTalk()
    {
        lightAttackTutorial.color = completeColor;
        lightAttackTitleTutorial.color = completeColor;
        heavyAttackTitleTutorial.color = activeColor;
        heavyAttackTutorial.color = activeColor;
        anim.CrossFade(StringData.IsTalking02, 0.2f);
        DialogueManager.Instance.AddNewDialogue(heavyAttackTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerHeavyAttack;
    }

    private void LockOnTrainingTalk()
    {
        heavyAttackTitleTutorial.color = completeColor;
        heavyAttackTutorial.color = completeColor;
        LockOnTitleTutorial.color = activeColor;
        LockOnTutorial.color = activeColor;
        anim.CrossFade(StringData.IsTalking02, 0.2f);
        DialogueManager.Instance.AddNewDialogue(lockOnTargetTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerLockOn;
    }

    private void MagicAttackTrainingTalk()
    {
        LockOnTitleTutorial.color = completeColor;
        LockOnTutorial.color = completeColor;
        magicAttackTitleTutorial.color = activeColor;
        magicAttackTutorial.color = activeColor;
        anim.CrossFade(StringData.IsTalking02, 0.2f);
        DialogueManager.Instance.AddNewDialogue(magicAttackTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerMagicAttack;
    }

    private void SparCombatTrainingTalk()
    {
        magicAttackTitleTutorial.color = completeColor;
        magicAttackTutorial.color = completeColor;
        anim.CrossFade(StringData.IsTalking02, 0.2f);
        DialogueManager.Instance.AddNewDialogue(sparCombatTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerProceedToSparring;
    }

    private void FinalTrainingTalk()
    {
        anim.CrossFade(StringData.IsTalking02, 0.2f);
        DialogueManager.Instance.AddNewDialogue(finalTrainingDialogue);
        DialogueManager.Instance.OnDialogueCompleteEvent += AllowPlayerToFinishTutorial;
    }

    private void AllowPlayerMovement()
    {
        PlayerControls.Instance.canMove = true;
    }

    private void AllowPlayerSprint()
    {
        PlayerControls.Instance.canSprint = true;
    }

    private void AllowPlayerDive()
    {
        PlayerControls.Instance.canDive = true;
    }

    private void AllowPlayerLightAttack()
    {
        PlayerControls.Instance.canLightAttack = true;
    }

    private void AllowPlayerHeavyAttack()
    {
        PlayerControls.Instance.canHeavyAttack = true;
    }

    private void AllowPlayerLockOn()
    {
        PlayerControls.Instance.canLockOn = true;
    }

    private void AllowPlayerMagicAttack()
    {
        PlayerControls.Instance.canMagicAttack = true;
    }

    private void AllowPlayerProceedToSparring()
    {
        leftDoorAnim.SetBool(StringData.OpenLeft, true);
        rightDoorAnim.SetBool(StringData.OpenRight, true);
        TutorialManager.Instance.sparrDummy.engagePlayer = true;
    }

    private void AllowPlayerToFinishTutorial()
    {
        TutorialManager.Instance.tutorialComplete = true;
    }

    private void InitTutorialToDoList()
    {
        activeColor = Color.white;
        todoColor = sprintTitleTutorial.color;
        completeColor= magicAttackTitleTutorial.color;

        movementTitleTutorial.color = activeColor;
        movementTutorial.color = activeColor;
        sprintTitleTutorial.color = todoColor;
        sprintTutorial.color = todoColor;
        diveTitleTutorial.color = todoColor;
        diveTutorial.color = todoColor;
        lightAttackTitleTutorial.color = todoColor;
        lightAttackTutorial.color = todoColor;
        heavyAttackTitleTutorial.color = todoColor;
        heavyAttackTutorial.color = todoColor;
        LockOnTitleTutorial.color = todoColor;
        LockOnTutorial.color = todoColor;
        magicAttackTitleTutorial.color = todoColor;
        magicAttackTutorial.color = todoColor;
    }
}

public enum TrainingStages
{
    MovementTraining,
    SprintTraining,
    DiveTraining,
    LightAttackTraining,
    HeavyAttackTraining,
    LockOnTraining,
    MagicAttackTraining,
}
