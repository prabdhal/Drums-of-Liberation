using UnityEngine;

public class EndTutorialSceneTrigger : EndSceneTrigger
{
    public bool isMovementComplete = false;
    public bool isSwordComplete = false;
    public bool isMagicComplete = false;

    public bool IsTutorialComplete { get { return isMovementComplete && isSwordComplete && isMagicComplete; } }

    protected override bool SceneCompletionConditions()
    {
        if (IsTutorialComplete)
        {
            PlayerDataManager.TutorialFinished = true;
            PlayerDataManager.CurrentHealth = PlayerManager.Instance.Stats.playerMaxHealthIncreasePerLevel[PlayerDataManager.PlayerLevel];
            PlayerDataManager.CurrentMana = PlayerManager.Instance.Stats.playerMaxManaIncreasePerLevel[PlayerDataManager.PlayerLevel];
            PlayerDataManager.CurrentStamina = PlayerManager.Instance.Stats.playerMaxStaminaIncreasePerLevel[PlayerDataManager.PlayerLevel];
            return true;
        }

        Debug.Log("Enemies left: " + GameManager.Instance.Enemies.Count);
        return false;
    }
}
