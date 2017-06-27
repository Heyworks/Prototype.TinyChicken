using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelection : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 30), "Scene1"))
        {
            if (PhotonNetwork.connected)
                PhotonNetwork.Disconnect();
            SceneManager.LoadScene(3);
        }

        if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 30), "Scene2"))
        {
            if (PhotonNetwork.connected)
                PhotonNetwork.Disconnect();
            SceneManager.LoadScene(4);
        }

        if (GUI.Button(new Rect(Screen.width / 2 - 50, 0, 100, 30), "PvP"))
        {
            SceneManager.LoadScene(0);
        }
    }
}
