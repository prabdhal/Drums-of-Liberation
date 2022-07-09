using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] int fullRestoreCost = 50;
    [SerializeField] float costScalingPerLevel = 2f;
    public float FullRestoreCost { get { return fullRestoreCost * costScalingPerLevel; } }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            PlayerInventory.Instance.BuyFullRestore(FullRestoreCost);
            Debug.Log("Player bought full restore potion, now has " + PlayerManager.Instance.Gold + " gold left");
        }
    }
}
