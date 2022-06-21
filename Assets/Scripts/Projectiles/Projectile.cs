using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float speed = 500f;
    [SerializeField]
    private float range = 200f;

    private Vector3 startPos;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        float distanceTravelled = Vector3.Distance(startPos, transform.position);

        rb.AddForce(transform.forward * speed * Time.fixedDeltaTime);

        if (range != 0 && distanceTravelled >= range)
            Destroy(gameObject);
    }

    public void Init(float speed, float range)
    {
        this.speed = speed;
        this.range = range;
    }
}
