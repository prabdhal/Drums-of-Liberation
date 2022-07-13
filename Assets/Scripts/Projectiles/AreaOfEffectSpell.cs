using UnityEngine;

public class AreaOfEffectSpell : MonoBehaviour
{
    public delegate void OnHitDel(GameObject target);
    public event OnHitDel OnHitEvent;

    public delegate void OnHitPlayerDel();
    public event OnHitPlayerDel OnHitPlayerEvent;

    [SerializeField] float startTimer = 1f;
    private float curTimer = 0f;

    private bool applyHit = false;


    void Start()
    {
        curTimer = 1f;
    }

    private void Update()
    {
        if (curTimer <= 0)
        {
            applyHit = true;
            curTimer = startTimer;
        }
        else curTimer -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag) && applyHit)
        {
            OnHitPlayerEvent();
            applyHit = false;
            curTimer = startTimer;
        }
    }
}
