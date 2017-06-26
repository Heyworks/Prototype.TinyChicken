using UnityEngine;
using System.Collections;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{
    void Awake()
    {
        Debug.Log("SPAWN PLAYER");

        //if (photonView.isMine)
        //{
        //    gameObject.transform.SetLayerRecursively(LayerMask.NameToLayer("Player"));
        //}
        //else
        //{
        //    gameObject.transform.SetLayerRecursively(LayerMask.NameToLayer("Enemy"));
        //}
    }

    void Start ()   
    {		    	
    	transform.SendMessage ("IsLocalPlayer", photonView.isMine, SendMessageOptions.DontRequireReceiver);
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {       
        GameManager.AddPlayer(transform);
    }

    void OnDestroy()
    {
        GameManager.RemovePlayer(transform);
    }
}