using System.Collections;
using System.Collections.Generic;
using RavingBots.CartoonExplosion;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float lifeTime = 0.5f;
    [SerializeField]
    private CartoonExplosionFX explosion = null;
    [SerializeField]
    private GameObject rocket = null;
    [SerializeField]
    private GameObject trail = null;
    [SerializeField]
    private int durability = 1;

    private float dist = 10000;
    private Transform tr;
    private bool isExploding;

    public void SetDistance(float distance)
    {
        dist = distance;
    }

    private void OnEnable()
    {
        tr = transform;
    }

    private void Update()
    {
        if (!isExploding)
        {
            tr.position += tr.forward*speed*Time.deltaTime;
            dist -= speed*Time.deltaTime;
            if (dist < 0)
            {
                Boom();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isExploding && collision.gameObject.layer == LayerMask.NameToLayer("Arena"))
        {
            if (durability > 0)
            {
                var reflection = Vector3.Reflect(tr.forward, collision.contacts[0].normal);
                tr.forward = reflection;
                durability--;
            }
            else
            {
                Boom();
            }
        }

        if (!isExploding && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Boom();
            Destroy(collision.gameObject);
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (!isExploding && other.gameObject.layer == LayerMask.NameToLayer("Arena"))
    //    {
    //    }

    //    if (!isExploding && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //    {
    //        Boom();
    //        Destroy(other.gameObject);
    //    }
    //}

    private void Boom()
    {
        isExploding = true;
        rocket.SetActive(false);
        trail.SetActive(false);
        explosion.Play();
        Destroy(gameObject, explosion.Duration);
    }
}
