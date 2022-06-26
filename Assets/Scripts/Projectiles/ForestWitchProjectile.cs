using UnityEngine;

public class ForestWitchProjectile : Projectile
{
    [SerializeField] protected float rotateSpeed = 99999999f;

    protected override void Start()
    {
        Vector3 dir = (PlayerManager.Instance.transform.position - transform.position).normalized;
        dir.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotateSpeed);

        base.Start();
    }
}
