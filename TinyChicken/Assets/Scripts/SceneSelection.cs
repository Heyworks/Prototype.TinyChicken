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
            SceneManager.LoadScene(0);
        }

        if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 30), "Scene2"))
        {
            SceneManager.LoadScene(1);
        }
    }
}
