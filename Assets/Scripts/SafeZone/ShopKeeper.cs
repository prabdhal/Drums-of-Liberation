using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] GameObject shopPanelUI;
    [SerializeField] TextMeshProUGUI shopPanelFeedbackText;
    [SerializeField] Slider purchaseItemCountSlider; 
    [SerializeField] TextMeshProUGUI purchaseItemCountText;
    [SerializeField] TextMeshProUGUI itemDescription;
    [SerializeField] int[] fullRestoreCost;

    
    [SerializeField] GameObject firstSelectGameObject;
    public float FullRestoreCost { get { return fullRestoreCost[PlayerManager.Instance.Stats.playerLevel]; } }

    private bool interact = false;

    private void Start()
    {
        PlayerControls.Instance.OnInteractEvent += Interact;

        if (playerInventory == null)
            playerInventory = PlayerManager.Instance.GetComponent<PlayerInventory>();
        if (shopPanelUI == null)
            shopPanelUI = GameObject.FindGameObjectWithTag(StringData.ShopPanelUI).GetComponent<GameObject>();
        purchaseItemCountSlider.wholeNumbers = true;
        purchaseItemCountText.text = purchaseItemCountSlider.value.ToString();
        shopPanelFeedbackText.text = null;
        itemDescription.text = ItemDescription();

        shopPanelUI.SetActive(false);
    }

    private void Update()
    {
        UpdatePurchaseItemCount();
    }

    private void Interact()
    {
        if (!shopPanelUI.activeInHierarchy)
        interact = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            if (interact)
            {
                OpenShopPanel();
                interact = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            CloseShopPanel();
        }
    }

    private void OpenShopPanel()
    {
        shopPanelUI.SetActive(true);
        shopPanelFeedbackText.text = null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectGameObject);
        Time.timeScale = 0f;
    }

    private void CloseShopPanel()
    {
        shopPanelUI.SetActive(false);
        shopPanelFeedbackText.text = null;
        Time.timeScale = 1f;
    }

    public void ShopCancelButton()
    {
        CloseShopPanel();
    }

    public void ShopPurchaseButton()
    {
        int itemCount = (int)purchaseItemCountSlider.value;
        if (!playerInventory.BuyFullRestore(itemCount, FullRestoreCost))
        {
            shopPanelFeedbackText.text = "You either do not have enough gold to spend or you are buying too many potion(s). You can only store 3 total potions!";
        }
        else
            shopPanelFeedbackText.text = null;
    }

    private string ItemDescription()
    {
        return "Full restore potion costs " + FullRestoreCost + " and restore " + playerInventory.baseHealthRestoreAmount[PlayerManager.Instance.Stats.playerLevel] + " points in health "
                                                              + ", " + playerInventory.baseManaRestoreAmount[PlayerManager.Instance.Stats.playerLevel] + " points in mana "
                                                              + ", and " + playerInventory.baseStaminaRestoreAmount[PlayerManager.Instance.Stats.playerLevel] + " points in stamina.";
    }

    public void UpdatePurchaseItemCount()
    {
        purchaseItemCountText.text = purchaseItemCountSlider.value.ToString();
    }
}
