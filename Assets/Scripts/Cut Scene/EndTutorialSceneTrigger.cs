using UnityEngine;

public class EndTutorialSceneTrigger : EndSceneTrigger
{
    private bool isMovementComplete = false;
    private bool isSprintingComplete = false;
    private bool isDivingComplete = false;
    private bool isLightAttackingComplete = false;
    private bool isHeavyAttackingComplete = false;
    private bool isLockOnComplete = false;
    private bool isMagicAttackingComplete = false;
    private bool isSparringComplete = false;
    private bool isfinalTalkComplete = false;

    public bool IsTutorialComplete
    {
        get
        {
            return isMovementComplete && isSprintingComplete && isDivingComplete && isLightAttackingComplete && 
                isHeavyAttackingComplete && isLockOnComplete && isMagicAttackingComplete && isSparringComplete && isfinalTalkComplete;
        }
    }


    private void Start()
    {
        TutorialManager.Instance.OnMovementTutorialComplete += MovementTutorialComplete;
        TutorialManager.Instance.OnSprintTutorialComplete += SprintTutorialComplete;
        TutorialManager.Instance.OnDiveTutorialComplete += DiveTutorialComplete;
        TutorialManager.Instance.OnLightAttackTutorialComplete += LightAttackTutorialComplete;
        TutorialManager.Instance.OnHeavyAttackTutorialComplete += HeavyAttackTutorialComplete;
        TutorialManager.Instance.OnLockOnTutorialComplete += LockOnTutorialComplete;
        TutorialManager.Instance.OnMagicAttackTutorialComplete += MagicAttackTutorialComplete;
        TutorialManager.Instance.OnSparCombatTutorialComplete += SparCombatTutorialComplete;
        TutorialManager.Instance.OnFinalTutorialComplete += FinalTalkTutorialComplete;
    }

    protected override bool SceneCompletionConditions()
    {
        Debug.Log("Is tutorial complete " + IsTutorialComplete);
        if (IsTutorialComplete)
        {
            PlayerDataManager.TutorialFinished = true;
            PlayerDataManager.CurrentHealth = PlayerManager.Instance.Stats.playerMaxHealthIncreasePerLevel[PlayerDataManager.PlayerLevel];
            PlayerDataManager.CurrentMana = PlayerManager.Instance.Stats.playerMaxManaIncreasePerLevel[PlayerDataManager.PlayerLevel];
            PlayerDataManager.CurrentStamina = PlayerManager.Instance.Stats.playerMaxStaminaIncreasePerLevel[PlayerDataManager.PlayerLevel];
            return true;
        }

        return false;
    }

    private void MovementTutorialComplete()
    {
        isMovementComplete = true;
    }

    private void SprintTutorialComplete()
    {
        isSprintingComplete = true;
    }

    private void DiveTutorialComplete()
    {
        isDivingComplete = true;
    }

    private void LightAttackTutorialComplete()
    {
        isLightAttackingComplete = true;
    }

    private void HeavyAttackTutorialComplete()
    {
        isHeavyAttackingComplete = true;
    }

    private void LockOnTutorialComplete()
    {
        isLockOnComplete = true;
    }

    private void MagicAttackTutorialComplete()
    {
        isMagicAttackingComplete = true;
    }

    private void SparCombatTutorialComplete()
    {
        isSparringComplete = true;
    }

    private void FinalTalkTutorialComplete()
    {
        isfinalTalkComplete = true;
    }
}
