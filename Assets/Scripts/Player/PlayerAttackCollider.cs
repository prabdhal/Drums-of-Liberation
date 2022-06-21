using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private PlayerCombat combat;

    public delegate void OnAttackDel(EnemyManager enemy);
    public event OnAttackDel OnAttackEvent;

    private void Start()
    {
        combat = GameObject.FindGameObjectWithTag(StringData.PlayerTag).GetComponentInChildren<PlayerCombat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringData.EnemyTag))
        {
            if (GameManager.Instance.Enemies.TryGetValue(other.name, out EnemyManager enemy))
            {
                OnAttackEvent?.Invoke(enemy);
                enemy.PlayerIsDetected(true);
                // add particle effects 
            }
        }
    }
}
