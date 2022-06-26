using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    public delegate void OnAttackDel(EnemyManager enemy);
    public event OnAttackDel OnAttackEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringData.EnemyTag))
        {
            if (GameManager.Instance.Enemies.TryGetValue(other.name, out EnemyManager enemy))
            {
                OnAttackEvent?.Invoke(enemy);
                enemy.GetHitDirection(transform);
                enemy.PlayerIsDetected(true);
                // add particle effects 
                
            }
        }
    }
}
