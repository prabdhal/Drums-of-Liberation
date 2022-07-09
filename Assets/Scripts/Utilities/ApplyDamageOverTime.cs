using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDamageOverTime : MonoBehaviour
{
    private bool isInMagma = false;
    [SerializeField] float burnDamageOverTime = 5f;
    [SerializeField] float burnDuration = 3f;
    [SerializeField] float burnInterval = 0.5f;
    [SerializeField] float magmaDamage = 10f;
    [SerializeField] float startInterval = 1f;
    private float curInterval = 5f;

    private void Start()
    {
        isInMagma = false;
        curInterval = 0f;
    }

    private void Update()
    {
        if (isInMagma)
        {
            if (curInterval <= 0)
            {
                curInterval = startInterval;
                PlayerManager.Instance.TakeDamage(magmaDamage);
                GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
                go.GetComponent<DamagePopup>().Init(magmaDamage, DamageType.Burn);
            }
            else
                curInterval -= Time.deltaTime;
        }
        else
            curInterval = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            isInMagma = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            isInMagma = false;
        }
    }
}
