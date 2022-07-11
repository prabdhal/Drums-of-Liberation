using TMPro;
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

    [SerializeField] TextMeshProUGUI fullRestoreCountText;

    [SerializeField] int maxFullRestoreCount = 3;
    public int curFullRestoreCount = 0;
    public float[] baseHealthRestoreAmount;
    public float[] baseManaRestoreAmount;
    public float[] baseStaminaRestoreAmount;


    public void Start()
    {
        if (fullRestoreCountText == null)
            fullRestoreCountText = GameObject.FindGameObjectWithTag(StringData.ItemCountText).GetComponent<TextMeshProUGUI>();
        PlayerControls.Instance.OnUseItemEvent += UseFullRestore;
        UpdateFullRestoreCountText();
    }

    public bool BuyFullRestore(int count, float cost)
    {
        float totalCost = count * cost;

        if (PlayerManager.Instance.Gold < totalCost || curFullRestoreCount + count > maxFullRestoreCount) return false;

        PlayerManager.Instance.Gold -= cost;
        AddFullRestore(count);
        PlayerManager.Instance.UpdateGoldTextUI();
        return true;
    }

    public void AddFullRestore(int count)
    {
        if (curFullRestoreCount >= maxFullRestoreCount) return;

        curFullRestoreCount += count;
        UpdateFullRestoreCountText();
    }

    public void UseFullRestore()
    {
        if (curFullRestoreCount <= 0) return;
        curFullRestoreCount--;
        UpdateFullRestoreCountText();

        PlayerManager.Instance.Stats.CurrentHealth += baseHealthRestoreAmount[PlayerManager.Instance.Stats.playerLevel];
        PlayerManager.Instance.Stats.CurrentMana += baseManaRestoreAmount[PlayerManager.Instance.Stats.playerLevel];
        PlayerManager.Instance.Stats.CurrentStamina += baseStaminaRestoreAmount[PlayerManager.Instance.Stats.playerLevel];
        PlayerManager.Instance.Stats.UpdateUI(true, true, true, true);
    }

    private void UpdateFullRestoreCountText()
    {
        fullRestoreCountText.text = curFullRestoreCount.ToString();
    }
}
