using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    public GameObject shadowAura;

    public delegate void OnAttackDel(EnemyManager enemy);
    public event OnAttackDel OnAttackEvent;

    [Header("Sword Audio")]
    [SerializeField] AudioSource swordSwing;
    [SerializeField] AudioSource swordImpact;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringData.EnemyTag))
        {
            if (GameManager.Instance.Enemies.TryGetValue(other.name, out EnemyManager enemy))
            {
                OnAttackEvent?.Invoke(enemy);
                enemy.PlayerIsDetected(true);
                enemy.GetHitDirection(transform);
                enemy.BloodEffect(transform);
                swordImpact.Play();
            }
        }
    }
}
