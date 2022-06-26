using UnityEngine;

public class ShadowBallProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();

        Debug.Log("Speed: " + speed);
        if (PlayerManager.Instance.lockOnTarget != null)
        {
            Vector3 dir = (new Vector3(PlayerManager.Instance.lockOnTarget.transform.position.x, transform.position.y, PlayerManager.Instance.lockOnTarget.transform.position.z) - transform.position).normalized;
            rb.AddForce(dir * speed * Time.deltaTime, ForceMode.VelocityChange);
        }
        else
            rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Init(float speed, float range)
    {
        base.Init(speed, range);
    }
}
