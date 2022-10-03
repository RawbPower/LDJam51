using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public GameObject explosion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject damageSource = collision.transform.parent.gameObject;
        GameObject hitObject = transform.parent.gameObject;
        Bullet bullet = damageSource.GetComponent<Bullet>();

        if (bullet != null && hitObject.CompareTag(bullet.ownerTag))
        {
            return;
        }

        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        Destroy(transform.parent.gameObject);
    }
}
