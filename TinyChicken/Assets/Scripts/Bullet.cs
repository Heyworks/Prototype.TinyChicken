using System.Collections;
using System.Collections.Generic;
using RavingBots.CartoonExplosion;
using UnityEngine;

public class Bullet : Photon.MonoBehaviour
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
                StartCoroutine(Boom());
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
                StartCoroutine(Boom());
            }
        }

        if (!isExploding && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            StartCoroutine(Boom());
            Destroy(collision.gameObject);
        }

        if (PhotonNetwork.connected && !isExploding && collision.gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject.GetComponent<PhotonView>().ownerId != photonView.ownerId)
        {
            StartCoroutine(Boom());
        }
    }

    private IEnumerator Boom()
    {
        isExploding = true;
        rocket.SetActive(false);
        trail.SetActive(false);
        explosion.Play();

        yield return new WaitForSeconds(explosion.Duration);

        if (PhotonNetwork.connected)
        {
            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
