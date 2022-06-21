using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField]
    private string targetTag = StringData.PlayerTag;

    public delegate void ApplyDamage();
    public event ApplyDamage OnApplyDamageEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetTag))
            OnApplyDamageEvent?.Invoke();
    }
}
