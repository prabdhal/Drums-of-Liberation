using UnityEngine;

public class RestorePlayerStats : MonoBehaviour
{
    [SerializeField] bool increaseHealth;
    [SerializeField] float healthRateOfIncrease = 50f;
    [SerializeField] bool increaseStamina;
    [SerializeField] float staminaRateOfIncrease = 50f;
    [SerializeField] bool increaseMana;
    [SerializeField] float manaRateOfIncrease = 50f;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            if (increaseHealth)
                PlayerManager.Instance.Stats.CurrentHealth += healthRateOfIncrease * Time.deltaTime;
            if (increaseStamina)
                PlayerManager.Instance.Stats.CurrentStamina += staminaRateOfIncrease * Time.deltaTime;
            if (increaseMana)
                PlayerManager.Instance.Stats.CurrentMana += manaRateOfIncrease * Time.deltaTime;

            PlayerManager.Instance.Stats.UpdateUI(true, true, true, false);
        }
    }

}
