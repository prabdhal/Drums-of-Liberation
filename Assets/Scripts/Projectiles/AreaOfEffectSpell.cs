using UnityEngine;

public class AreaOfEffectSpell : MonoBehaviour
{
    public delegate void OnHitDel(GameObject target);
    public event OnHitDel OnHitEvent;

    public delegate void OnHitPlayerDel();
    public event OnHitPlayerDel OnHitPlayerEvent;

    [SerializeField] float startTimer = 1f;
    private float curTimer = 0f;


    void Start()
    {
        curTimer = 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            if (curTimer <= 0)
            {
                OnHitPlayerEvent();
                curTimer = startTimer;
            }
            else curTimer -= Time.deltaTime;
        }
    }
}
