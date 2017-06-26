// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

/*
This class can be used like an interface.
Inherit from it to define your own movement motor that can control
the movement of characters, enemies, or other entities.
*/

public class MovementMotor : Photon.MonoBehaviour
{
    // The direction the character wants to move in, in world space.
    // The vector should have a length between 0 and 1.
    [HideInInspector] public Vector3 movementDirection;

    // Simpler motors might want to drive movement based on a target purely
    [HideInInspector] public Vector3 movementTarget;

    // The direction the character wants to face towards, in world space.
    [HideInInspector] public Vector3 facingDirection;

    protected void OnPhotonSerializeViewBase(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data			
            correctPlayerPos = (Vector3) stream.ReceiveNext();
            correctPlayerRot = (Quaternion) stream.ReceiveNext();
        }
    }

    //
    //  TODO: fix constant movement!
    //
    //

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    void Update()
    {
        if (!photonView.isMine)
        {
            if (correctPlayerPos == Vector3.zero) return;

            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            if (Vector3.Distance(correctPlayerPos, transform.position) < 4)
            {
                transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime*5);
                transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime*5);
            }
            else
            {
                transform.position = correctPlayerPos;
                transform.rotation = correctPlayerRot;
            }
        }
    }
}