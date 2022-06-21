using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField]
    private float deathTimer = 10f;

    [SerializeField]
    private Tags[] collisionTags;
    private List<Tags> tags = new List<Tags>();

    [SerializeField]
    private GameObject collisionEffect;

    private float curTimer;

    public delegate void OnHitDel(GameObject target);
    public event OnHitDel OnHitEvent;


    private void Start()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.transform.name);
        Debug.Log("Collided with " + other.tag);
        if (other.CompareTag(StringData.PlayerTag) && tags.Contains(Tags.Player) ||
          other.GetComponent<Collider>().CompareTag(StringData.EnemyTag) && tags.Contains(Tags.Enemy))
        {
            OnHitEvent(other.gameObject);
            Instantiate(collisionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        if (other.CompareTag(StringData.Obstacle) && tags.Contains(Tags.Obstacle))
        {
            Instantiate(collisionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}