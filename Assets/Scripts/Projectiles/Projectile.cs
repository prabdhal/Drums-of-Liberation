using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected AudioSource effectAudio;
    [SerializeField]
    protected Rigidbody rb;
    [SerializeField]
    protected SphereCollider col;

    protected float speed = 300f;
    protected float range = 50f;

    protected Vector3 startPos;

    [SerializeField] float deathTimer = 5f;

    [SerializeField] Tags[] collisionTags;
    private List<Tags> tags = new List<Tags>();
    [SerializeField] LayerMask detectLayer;

    [SerializeField] GameObject collisionEffect;
    [SerializeField] AudioSource collisionAudio;

    private float curTimer;

    [SerializeField] bool useRaycastCollision = false;

    public delegate void OnHitDel(GameObject target);
    public event OnHitDel OnHitEvent;

    public delegate void OnHitPlayerDel();
    public event OnHitPlayerDel OnHitPlayerEvent;

    [SerializeField] bool isPlayer = false;


    protected virtual void Start()
    {
        if (effectAudio == null)
            effectAudio = GetComponent<AudioSource>();
        effectAudio.volume = AudioManager.Instance.SoundVolume;
        effectAudio.mute = AudioManager.Instance.Mute;
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (col == null)
            col = GetComponent<SphereCollider>();
        if (collisionAudio == null)
            collisionAudio = collisionEffect.GetComponent<AudioSource>();

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.useGravity = false;
        startPos = transform.position;
        curTimer = deathTimer;

        for (int i = 0; i < collisionTags.Length; i++)
        {
            tags.Add(collisionTags[i]);
        }
    }

    private void Update()
    {
        if (curTimer <= 0)
            Destroy(gameObject);
        else
            curTimer -= Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
        float distanceTravelled = Vector3.Distance(startPos, transform.position);

        rb.AddForce(transform.forward * speed * Time.fixedDeltaTime);

        if (range != 0 && distanceTravelled >= range)
            Destroy(gameObject);
    }

    public virtual void Init(float speed, float range)
    {
        this.speed = speed;
        this.range = range;
    }

    protected virtual void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag(StringData.PlayerTag) && tags.Contains(Tags.Player))
        {
            ContactPoint contact = col.contacts[0];

            OnHitPlayerEvent();
            Instantiate(collisionEffect, contact.point, transform.rotation);
            collisionAudio.Play();
            Destroy(gameObject);
        }
        else if (col.collider.CompareTag(StringData.EnemyTag) && tags.Contains(Tags.Enemy))
        {
            ContactPoint contact = col.contacts[0];

            OnHitEvent(col.gameObject);
            if (GameManager.Instance.Enemies.TryGetValue(col.collider.name, out EnemyManager enemy))
            {
                enemy.PlayerIsDetected(true);
                enemy.GetHitDirection(transform);
                //enemy.BloodEffect(transform);
            }
            Instantiate(collisionEffect, contact.point, transform.rotation);
            collisionAudio.Play();
            Destroy(gameObject);
        }
        else if (col.collider.CompareTag(StringData.Obstacle) && tags.Contains(Tags.Obstacle))
        {
            ContactPoint contact = col.contacts[0];

            Instantiate(collisionEffect, contact.point, transform.rotation);
            collisionAudio.Play();
            Destroy(gameObject);
        }
    }

    //private void DetectCollisionViaRayCast()
    //{
    //    Debug.Log("using Raycast");
    //    RaycastHit hit;
    //    Vector3 origin = transform.position;
    //    Vector3 dir = transform.TransformDirection(Vector3.forward);

    //    if (Physics.Raycast(origin, dir, out hit, Mathf.Infinity, detectLayer))
    //    {
    //        Debug.DrawRay(origin, dir, Color.red);

    //        if (detectLayer.Equals(StringData.PlayerTag))
    //        {
    //            OnHitPlayerEvent();
    //            Instantiate(collisionEffect, hit.point, transform.rotation);
    //            Destroy(gameObject);
    //        }
    //        else if (detectLayer.Equals(StringData.EnemyTag))
    //        {
    //            OnHitEvent(hit.collider.gameObject);
    //            Instantiate(collisionEffect, hit.point, transform.rotation);
    //            Destroy(gameObject);
    //        }
    //        else if (detectLayer.Equals(StringData.Obstacle))
    //        {
    //            Instantiate(collisionEffect, hit.point, transform.rotation);
    //            Destroy(gameObject);
    //        }
    //    }
    //}
}
