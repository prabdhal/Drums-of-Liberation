using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLockDetection : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();

    [SerializeField]
    private float maxDistance = 1000f;
    [SerializeField]
    private float lockViewDistance = 50f;

    private float closestDistance;


    private void Update()
    {
        if (PlayerManager.Instance.TargetLock)
        {
            if (CurrentTargetOutsideOfRange(PlayerManager.Instance.lockOnTarget.transform))
            {
                PlayerManager.Instance.TargetLock = false;
                PlayerManager.Instance.lockOnTarget = null;
                PlayerManager.Instance.OnLock(null);
            }
        }

        targets.RemoveAll(e => e.tag.Equals(StringData.Untagged));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringData.EnemyTag))
        {
            targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringData.EnemyTag))
        {
            targets.Remove(other.gameObject);
        }
    }

    private bool CurrentTargetOutsideOfRange(Transform target)
    {
        if (target == null) return true;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        return distance > lockViewDistance;
    }

    public GameObject GetClosestTarget()
    {
        GameObject tar = null;
        if (targets.Count <= 0)
            return null;
        else if (targets.Count == 1)
            return targets[0];
        else if (targets.Count > 1)
        {
            foreach (GameObject target in targets)
            {
                closestDistance = maxDistance;

                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance < closestDistance && distance <= lockViewDistance)
                {
                    closestDistance = distance;
                    tar = target;
                }
            }
        }
        return tar;
    }
}
