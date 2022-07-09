using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    #region Singleton
    public static PlayerInventory Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    #endregion

    [SerializeField] int maxFullRestoreCount = 3;
    private int fullRestoreCount = 0;
    [SerializeField] float baseHealthRestoreAmount = 50f;
    [SerializeField] float baseManaRestoreAmount = 75f;
    [SerializeField] float baseStaminaRestoreAmount = 100f;


    public void Start()
    {

    }

    public void BuyFullRestore(float cost)
    {
        if (PlayerManager.Instance.Gold < cost || fullRestoreCount >= maxFullRestoreCount) return;

        PlayerManager.Instance.Gold -= cost;
        AddFullRestore();
    }

    public void AddFullRestore()
    {
        if (fullRestoreCount >= maxFullRestoreCount) return;

        fullRestoreCount++;
    }

    public void UseFullRestore()
    {
        if (fullRestoreCount <= 0) return;
        fullRestoreCount--;

        PlayerManager.Instance.Stats.CurrentHealth += baseHealthRestoreAmount * PlayerManager.Instance.Stats.playerLevel;
        PlayerManager.Instance.Stats.CurrentMana += baseManaRestoreAmount * PlayerManager.Instance.Stats.playerLevel;
        PlayerManager.Instance.Stats.CurrentStamina += baseStaminaRestoreAmount * PlayerManager.Instance.Stats.playerLevel;
    }
}
