using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Photon.MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private Transform tankHead;
    [SerializeField]
    private GameObject bulletPrefab;

    private Joystick joystick;

    private void Start()
    {
        if ((PhotonNetwork.connected && photonView.isMine) || !PhotonNetwork.connected)
        {
            joystick = GameObject.FindGameObjectWithTag("LeftStick").GetComponent<Joystick>();
            joystick.PointWasSet += Joystick_PointWasSet;
        }
    }

    private void Joystick_PointWasSet(Vector3 shootPosition)
    {
        Fire(shootPosition);
        //photonView.RPC("Fire", PhotonTargets.Others, shootPosition);
    }

    [PunRPC]
    private void Fire(Vector3 shootPosition)
    {
        var headLookDirection = new Vector3(shootPosition.x, tankHead.position.y, shootPosition.z);
        headLookDirection.y = tankHead.position.y;
        tankHead.LookAt(headLookDirection);
        GameObject go;
        if (PhotonNetwork.connected)
        {
            go = PhotonNetwork.Instantiate(bulletPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
        }
        else
        {
            go = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        var bullet = go.GetComponent<Bullet>();

        // Find the object hit by the raycast
        RaycastHit hitInfo = new RaycastHit();
        Physics.Raycast(transform.position, spawnPoint.forward, out hitInfo, 100, 1 << LayerMask.NameToLayer("Arena"));
        if (hitInfo.transform)
        {
            bullet.SetDistance(hitInfo.distance);
            bullet.SetDistance(50);
        }
        else
        {
            bullet.SetDistance(50);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(tankHead.rotation);
        }
        else
        {
            //Network player, receive data			
            tankHead.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
