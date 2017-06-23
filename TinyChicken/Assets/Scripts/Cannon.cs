using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
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
        joystick = GameObject.FindGameObjectWithTag("LeftStick").GetComponent<Joystick>();
        joystick.PointWasSet += Joystick_PointWasSet;
    }

    private void Joystick_PointWasSet(Vector3 shootPosition)
    {
        var headLookDirection = new Vector3(shootPosition.x, tankHead.position.y, shootPosition.z);
        headLookDirection.y = tankHead.position.y;
        tankHead.LookAt(headLookDirection);
        var go = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        var bullet = go.GetComponent<Bullet>();

        // Find the object hit by the raycast
        RaycastHit hitInfo = new RaycastHit();
        Physics.Raycast(transform.position, spawnPoint.forward, out hitInfo, 100, 1 << LayerMask.NameToLayer("Arena"));
        if (hitInfo.transform)
        {
            bullet.SetDistance(hitInfo.distance);
            bullet.SetDistance(100);
        }
        else
        {
            bullet.SetDistance(100);
        }
    }
}
