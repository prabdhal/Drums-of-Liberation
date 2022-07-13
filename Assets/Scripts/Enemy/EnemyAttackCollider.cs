using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField]
    private string targetTag = StringData.PlayerTag;

    public delegate void ApplyDamage();
    public event ApplyDamage OnApplyDamageEvent;

    public bool continuousHit = false;

    [SerializeField] float attackPerUnitOfTime = 1f;
    private float timer;

    private bool applyHit = false;

    private void OnEnable()
    {
        timer = attackPerUnitOfTime;
    }

    private void Update()
    {
        if (timer <= 0)
        {
            applyHit = true;
            timer = attackPerUnitOfTime;
        }
        else
            timer -= Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetTag))
            OnApplyDamageEvent?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!continuousHit) return;

        if (other.tag.Equals(targetTag) && applyHit)
        {
            OnApplyDamageEvent?.Invoke();
            applyHit = false;
            timer = attackPerUnitOfTime;
        }
    }
}
